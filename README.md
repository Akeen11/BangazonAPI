# Welcome to :boom: Bangazon :boom:

----
## What is Bangazon? :dollar:
Bangazon is an e-commerce store where users are able to buy and sell products. Think Etsy meets Craigslist. 

----
## Software Dependencies :space_invader:

[SSMS](https://docs.microsoft.com/en-us/sql/ssms/download-sql-server-management-studio-ssms?view=sql-server-2017), 
[Visual Studio](https://visualstudio.microsoft.com/),
and [Postman](https://www.getpostman.com/).

---
## Installation :computer:
1. Clone repository
```
git clone https://github.com/NSS-Likeable-Lemurs/BangazonAPI.git 
```
2. Open SSMS and make a database using the Bangazon.sql folder
3. Open visual studio, click view and open solution explorer
4. Replace info in appsettings.Dependencies.json with template from appsettingstemplate.text
5. Rename appsettings.Dependencies.json to appsettings.json
6. Add connection string to appsettings.json
- Click view
- Select sequel server object explorer
- Click the server that begins with Desktop
- Click the databases folder and right click the database you will use
- At the bottom of the pop-up, click the properties link
- Select and copy the connection string. It should be the ninth option down
- Put the connection string in the appsettings.json as the value for the Default Connection key
- Replace DataSource= with Server= 
7. See links below on how to test different functionalities for the controller through postman

----
## API calls :link:
1. Get all data from a controller
```
http://localhost:5000/api/[name]
```
2. Get data by a specific id
```
http://localhost:5000/api/[name]/[id]
```
3. Get data by a specific data string
```
http://localhost:5000/api/[name]?q=[string]
```
4. Post data
```
http://localhost:5000/api/[name]
```
- Select Headers tab. Add Content-Type to Key and application/json to Value
- Select Body tab. Create object to add to the data
5. Put data
```
http://localhost:5000/api/[name]/[id]
```
- Select Headers tab. Add Content-Type to Key and application/json to Value
- Select Body tab. Modify the values of the object you want to edit.
6. Delete data 
```
http://localhost:5000/api/[name]/[id]
```


----
![CustomerERD](https://github.com/NSS-Likeable-Lemurs/BangazonAPI/blob/master/Images/Bangazon-Customer.PNG)
![CompanyERD](https://github.com/NSS-Likeable-Lemurs/BangazonAPI/blob/master/Images/Bangazon-Company.PNG)