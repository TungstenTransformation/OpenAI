# OpenAI & ChatGPT Integration for Kofax Transformation
samples of using OpenAI and ChatGPT in Kofax Transformation.


## Register at OpenAI.com

## Generate an API key
* Open https://platform.openai.com/account/api-keys
* Click **+ Create new secret key**.
* Copy the secret key to paste into Kofax Transformation.

## Configure Kofax Transformation

* Add a variable OpenAI-key.
* Add script locator, starting with "OpenAI_".
* Add the key value pairs that you expect
* Add the [script](OpenAI.vb) to the document class. The script uses the OpenAI [REST API](https://platform.openai.com/docs/api-reference/introduction).
* Press Test on the Locator.

Let me know any useful examples you find.

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
please parse "please transfer forty thousand euros to Peter Brown's bank account AT483200000012345864"

Request: Transfer forty thousand euros (€40,000)
Payee: Peter Brown 
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
