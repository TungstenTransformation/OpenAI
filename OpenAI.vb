Option Explicit

' Class script: Document

Private Sub Document_AfterLocate(ByVal pXDoc As CASCADELib.CscXDocument, ByVal LocatorName As String)
   'Call the customized table locator for OpenAI
   If InStr(LocatorName,"OpenAI") Then OpenAI_Extract(pXDoc, LocatorName)
End Sub


Private Sub SL_InputText_LocateAlternatives(ByVal pXDoc As CASCADELib.CscXDocument, ByVal pLocator As CASCADELib.CscXDocField)
   'This locator outputs all the words of the document
   Dim W As Long
   With pLocator.Alternatives.Create
      .Confidence=1
      For W =0 To pXDoc.Words.Count-1
         .Words.Append(pXDoc.Words(W))
      Next
   End With
End Sub

'================================
'===========OpenAI===============
'================================
Private Sub OpenAI_Extract(ByVal pXDoc As CASCADELib.CscXDocument, ByVal LocatorName As String)
   ' Read the text from the previous locator, and pass it to OpenAI. put the results into a Table
   Dim Locs As CscLocatorDefs, OpenAILoc As CscLocatorDef, Prompt As String, Results As String
   If pXDoc.ExtractionClass="" Then Err.Raise(4567,, "XDocument needs to be classified!")
   Set Locs=Project.ClassByName(pXDoc.ExtractionClass).Locators
   Set OpenAILoc=Locs.ItemByName(LocatorName)
   If OpenAILoc.FieldType <> CscExtractionFieldType.CscFieldTypeTable Then Err.Raise(4568,,LocatorName & " needs to be a table locator")
   If OpenAILoc.Index=0 Then Err.Raise (4569,, "There needs to be a locator previous to " & LocatorName & " to provide input")
   With pXDoc.Locators(OpenAILoc.Index-1).Alternatives 'get output of previous locator
      If .Count=0 Then Exit Sub ' there is no input text, so do nothing
      Prompt=.ItemByIndex(0).Text
   End With
   'Call Open AI with a model, input text, temperature and max tokens. https://platform.openai.com/docs/api-reference/completions/create
   Results=OpenAI_Cache("text-davinci-003", Prompt, pXDoc.FileName, 0, 2048)
   ' Put the results into a table
   With pXDoc.Locators.ItemByName(LocatorName).Alternatives(0)
      .Confidence=1
      Table_Parse(.Table,Results, "|")
   End With
End Sub

Sub Table_Parse(Table As CscXDocTable, Results As String, Optional Delim As String="|")
   'Parse the output of OpenAI into table rows and cells
   Dim Textlines() As String, textline As String, Row As CscXDocTableRow, C As Long, CellText As String
   Table.Rows.Clear
   For Each textline In Split(Results,vbNewLine)
      If InStr(textline,"|")>0 Then
         Set Row=Table.Rows.Append
         C=0
         For Each CellText In Split(textline,Delim)
            Row.Cells(C).Text=Trim(CellText)
            C=C+1
         Next
      End If
   Next
End Sub

Private Function OpenAI_Cache(Model As String, Prompt As String, Filename As String, Optional Temperature As Long=1, Optional MaxTokens As Long=2048) As String
   'this makes a copy of the OpenAI response as a .json file next to the xdoc. Saves time and costs by not calling OpenAI when not needed.
   'Delete the .json file if you want to call OpenAI again
   Dim Result As String, textline As String
   Filename=Replace(Filename,".xdc",".json")
   If Dir(Filename)="" Then ' test for file not existing
      Result=OpenAI_REST("text-davinci-003", Prompt, Temperature, MaxTokens)
      Open Filename For Output As #1
      Print #1, vbUTF8BOM & Result
      Close #1
   Else
      Open Filename For Input As #1
      Do While Not EOF(1)
         Line Input #1, textline
         Result = Result & textline & vbCrLf
      Loop
      Close #1
   End If
   'Extract just the text response from the results returns from OpenAI
   OpenAI_Cache=JSON_Extract(Result,"text")
End Function


Private Function OpenAI_REST(Model As String, Prompt As String, Optional Temperature As Long=1, Optional MaxTokens As Long=2048) As String
   'Call OpenAI. This returns a JSON object
   'On Menu/Edit/References... Add reference to "Microsoft XML, v 6.0" for HTTP
   'On Menu/Edit/References... Add reference to "Microsoft Scripting Runtime" for Dictionary
   Dim APIKey As String, HTTP As New MSXML2.XMLHTTP60, JSON As New Dictionary
   'See definition of temperature, maxTokens and available models here https://platform.openai.com/docs/api-reference/completions/create
   'Get your OpenAI API key https://platform.openai.com/account/api-keys and add to script variable "OpenAI Key"
   If Not Project.ScriptVariables.ItemExists("OpenAI Key") Then Err.Raise(4570,, "OpenAI Key needs to be in script variable 'OpenAI Key'")
   'Create JSON request to send to OpenAI
   JSON.Add("model", Model)
   JSON.Add("prompt", Prompt)
   JSON.Add("max_tokens", MaxTokens)
   JSON.Add("temperature", Temperature)
   'Call OpenAI
   APIKey=Project.ScriptVariables.ItemByName("OpenAI Key").Value
   HTTP.Open("POST", "https://api.openai.com/v1/completions",varAsync:=False)
   HTTP.setRequestHeader("Authorization", "Bearer " & APIKey)
   HTTP.setRequestHeader("Content-Type", "application/json")
   HTTP.send(JSON_String(JSON))
   If HTTP.status<>200 Then Err.Raise (4571,,"OpenAI Error: (" & HTTP.status & ") " & HTTP.responseText)
   OpenAI_REST = HTTP.responseText
End Function

'================================
'===========JSON=================
'================================
Function JSON_String(JSON As Dictionary) As String
   'Output JSON in normal text format
   Dim out As String, Key As String, Value
   For Each Key In JSON.Keys
      Select Case TypeName(JSON(Key))
      Case "String"
         Value="""" & JSON_Escape(JSON(Key)) & """"
      Case "Long"
         Value= CStr(JSON(Key))
      Case Else
         Value=JSON(Key)
      End Select
      out= out & vbTab & """" & Key & """: "  & Value & "," & vbCrLf
   Next
   JSON_String= "{" & vbCrLf & Left(out,Len(out)-3) & vbCrLf & "}"
End Function

Function JSON_Escape(a As String) As String
   'https://www.json.org/json-en.html
   a=Replace(a,"\","\\") 'backslash
   a=Replace(a,"/","\/") 'forward slash
   a=Replace(a,vbBack,"\b") 'backspace
   a=Replace(a,vbFormFeed,"\f") 'form feed
   a=Replace(a,vbNewLine,"\n") 'new line
   a=Replace(a,vbLf,"\r") 'carraige return
   a=Replace(a,vbTab,"\t") 'tab
   a=Replace(a,"""","\""") 'double quote
   JSON_Escape=a
End Function

Public Function JSON_Unescape(a As String) As String
   'https://www.json.org/json-en.html
   a=Replace(a,"\""","""") 'double quote
   a=Replace(a,"\\","\") 'backslash
   a=Replace(a,"\/","/") 'forward slash
   a=Replace(a,"\b",vbBack) 'backspace
   a=Replace(a,"\f",vbFormFeed) 'form feed
   a=Replace(a,"\n",vbNewLine) 'new line
   a=Replace(a,"\r",vbCr) 'carraige return
   a=Replace(a,"\t",vbTab) 'tab
   JSON_Unescape=a
End Function

Public Function JSON_Extract(JSON As String, element As String) As String
   'simple function to return a JSON element
   'Add reference to "Microsoft VBSCript Regular Expressions 5.5"
   Dim rex As New RegExp, Results As VBScript_RegExp_55.MatchCollection
   rex.Pattern="""" & element & """:\s*""(.*?)"""
   JSON=Replace(JSON,"\""","‼")   ' convert \" to ‼ to not confuse regex on "
   Set Results= rex.Execute(JSON)
   If Results.Count>0 Then
      JSON_Extract = JSON_Unescape(Replace(Results(0).SubMatches(0),"‼","\"""))
   Else
      Err.Raise(4789,, "failed to find " & element & " in JSON " & JSON)
   End If
End Function

