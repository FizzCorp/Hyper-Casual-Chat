# Hypercasual Input
Hypercasual Input sample is designed to demonstrate the usage of `FizzChatView` with Hypercasual Input View. `FizzHypercasualInputView` is used as a static keyboard which will show predefined phrases and sticker. 

## Hypercasual Input Data
Input view uses a static data file created by using ScriptableObjects.  There is a default data file also provided which is placed in Resources folder. You can add/remove as many phrases and sticker as you want. Phrases are already translated in few languages.

> Note: All data item id's should be unique and removing an item from data file can cause some view disturbance.
## Custom Views
To display phrases and sticker we  have added custom views to chat cells. Predefined messages use `data` property of message. The custom view are added to the cells by parsing data of chat messages.

> Note: Implement IFizzCustomMessageCellViewDataSource to parse message data and generate custom nodes in chat cells. Try Custom Cells Sample for more details.
 
## Configurations
To add chat view with predefined input to your game simply drag and drop the `FizzHypercasualChatView` prefab from Resources directory. It is configured to use both custom view and data file to run and you can add/remove or edit the data file at anytime.
