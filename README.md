# Keonn C\# REST API examples

&#8658; Please check our wiki: [https://wiki.keonn.com/software/advannet/development/rest-api-development/c-rest-development](https://wiki.keonn.com/software/advannet/development/rest-api-development/c-rest-development)

These examples make use of AdvanNet's REST API to show some of the potential functionalities that can be implemented with REST operations

* **ADRDAsynch:** Uses the TCP port 3177 to receive all read information from the reader
* **ADRDSynch:** Retrieves reader's data read during a single inventory operation
* **AEASynch:** Works with the reader's EPC_ALARM read mode using the TCP port 3177 to show when and which tags trigger alarms.
* **GPIEvents:** Uses the TCP port 3177 to show information of GPI events detected and fired by the reader.
* **ProcessAlarms:** Works with the reader's EPCBULK-EAS read mode using the TCP port 3177 to show when and which tags trigger alarms.
* **SystemConfig:** Uses the REST API to change the reader's date and time configuration
* **TestWriteEPC:** Example that shows how to perform some Tag operations such as Writing an EPC, writing an EPC via the Commission Tag Operation or killing a tag
* **Util:**  Bunch of classes used in the examples
