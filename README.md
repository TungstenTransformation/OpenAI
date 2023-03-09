# OpenAI's GPT-3 Integration for Kofax Transformation
How to integration [OpenAI](https://openai.com/)'s [GPT-3](https://openai.com/blog/gpt-3-apps) text model into Kofax Transformation for use in Kofax TotalAgility, Kofax RPA, Kofax Mobile Capture and Kofax Capture.  

This demo inputs this sample text to GPT-3's "most capable" model **text-davinci-003**.
```
    A table summarizing transfers: Please transfer forty five yen to 
    Sonya and 85,456.34€ to Tom's bank account 6546084
```  
 which returns  
```
    | Recipient | Amount | Currency | Bank Account |
    |-----------|--------|----------|--------------|  
    | Sonya     | 45     | Yen      | N/A          |  
    | Tom       | 85,456.34 | Euro    | 6546084      |
```
This is then put into a Kofax Transformation Table.  
![image](https://user-images.githubusercontent.com/103566874/224074940-fed89768-e4c2-46fc-9518-4062954836d0.png)

Note that OpenAI is a cloud-server based in USA, and the data may be used by them to train future models.  
Look at the bottom of this page for sample texts and responses.

## Why Integrate into Kofax Transformation?
* Partners and customers are already asking for it.
* Kofax Transformation is the **Intelligent Document Processing** differentiator that is the foundation of Kofax Intelligent Automation.
![image](https://user-images.githubusercontent.com/103566874/224083618-d8820a37-e552-4707-8828-37d1263d9630.png)

* Kofax Transformation is an AI platform. It provides
  * Testing.
  * Training and test set management.
  * Benchmarking.
  * Scripting.
  * Text formatting and field validation.
  * Integration with any other locators.
  * Human Validation interface.
* Supports TotalAgility, Kofax Capture, Kofax Mobile and Kofax RPA out-of-the-box.

## Register at OpenAI.com
* Create a free account at https://platform.openai.com/signup.
* OpenAI's GPT-3 offers [**chat**](https://platform.openai.com/docs/guides/chat) as in ChatGPT and [**text completions**](https://platform.openai.com/docs/guides/completion). We will use text completions in this demo, because we are giving an input and want an output, we are not making a dialog.
* Open [**text completions**](https://platform.openai.com/docs/guides/completion) and experiment (see down the bottom for examples).
* You have access numerous [models](https://platform.openai.com/docs/models). This demo uses GPT-3's **most capable** model **text-davinci-003**.
## Generate an OpenAI API key
* Open https://platform.openai.com/account/api-keys
* Click **+ Create new secret key**.
* Copy the secret key to paste into Kofax Transformation.

## Configure Kofax Transformation
* Download the [OpenAI](https://github.com/KofaxTransformation/OpenAI/tree/main/fpr) project from GitHub.  
* Open **Menu Project/Configuration/ScriptVariables** and paste your key into script variable **OpenAI Key**.
![image](https://user-images.githubusercontent.com/103566874/223722190-2225522f-e6fe-42db-8f74-035e6170ef79.png)
* Add script locator called **SL_InputText**. *This script locator just reads in the text of the sample document*.
* Create 5 new Columns in the Column Pool in **ProjectSettings/Tables**. This allows you to use a generic table to test various table formats.  
![image](https://user-images.githubusercontent.com/103566874/224077622-a8a3e65b-4a5e-4b33-8afc-bbacc32e4671.png)
* Create a Table model called **Generic** with the 5 columns.  
![image](https://user-images.githubusercontent.com/103566874/224077952-43c2154c-013c-4cbe-9c4d-91a69eef2528.png)

* Add the [script](OpenAI.vb) to the document class. The script uses the OpenAI [REST API](https://platform.openai.com/docs/api-reference/introduction).
* AIn Script Editor Menu **Edit/References...** add references to
    * Microsoft XML, v 6.0
    * Microsoft Scripting Runtime  
    * Microsoft VBSCript Regular Expressions 5.5  
    ![image](https://user-images.githubusercontent.com/103566874/224080390-e928a33f-dcb5-468b-a1cd-96928bbbf465.png)
 
* Press Classify & Extract (F7) on the [sample document](fpr/test/01.xdc).
* View results in **Extraction Results** Window.  
![image](https://user-images.githubusercontent.com/103566874/224080851-7446be1d-8866-4ce7-8b25-9a3b6674ea8b.png)


Let me know of any useful examples you find.

## Examples
```
please find full address and postcode in  "Deliver to 30 Parkview Rise, Hackham by 4pm monday"

Full Address: 30 Parkview Rise, Hackham, SA 5163
Postcode: SA 5163
```
```
please find street, town and postcode of Australian address in  "Deliver to 30 Parkview Rise Hackham by 4pm monday"

Street: Parkview Rise
Town: Hackham
Postcode: 5163
```
```
please find street, town and postcode  of "30 Parkview Rise Hackham"

Street: Parkview Rise
Town: Hackham
Postcode: 5163
```
```
please parse IBAN "AT483200000012345864"

Country: AT 
Bank and Branch Code: 4832                     <--- Note that this is incorrect!
Account Number: 000012345864
```
```
please parse "please transfer forty thousand euros to Peter Brooks' bank account AT483200000012345864"

Request: Transfer forty thousand euros (€40,000)
Payee: Peter Brooks 
Account Number: AT483200000012345864
```
```
A table summarizing transfers: Please transfer forty five yen to Sonya and 85,456.34€ to Tom's bank account 6546084

| Recipient | Amount | Currency | Bank Account |
|-----------|--------|----------|--------------|
| Sonya     | 45     | Yen      | N/A          |
| Tom       | 85,456.34 | Euro    | 6546084      |
```
```
extract the complaints and compliments from this review "I found bugs in the bed, the window was leaking and the heater didn't work, but i really enjoyed the view"

Complaints: 
- Found bugs in the bed
- Window was leaking
- Heater didn't work

Compliment: 
- Enjoyed the view
```
