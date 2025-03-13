# ProductManagement
# API Using Razor View 
1. Access using http://localhost:5014/
2. 
# API Functional Test using Swagger
1. run the project
2. go to this endpoint /api/v1/RefUser and then create user using this json example
  {
    "guid": null,
    "name": "Admin New",
    "username": "admin2",
    "email": "user@example.com",
    "isActive": true,
    "password": "admin1223"
  },  after filling the json then klik execute below
3. guid must as a flag create new user
4. after succcess register, go to this endpoint /api/v1/Auth/login, aand then input username and password then click execute below
5. if success login, then the result will be token like this example
  {
    "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxIiwidW5pcXVlX25hbWUiO"
  }
6. Then copy paste the token without quote, then clikc this button ![image](https://github.com/user-attachments/assets/2d1ae33c-47f9-412b-96d1-e908a4130b18)
7. then it will be show a popup like this ![image](https://github.com/user-attachments/assets/9e900340-16ca-457a-875c-070156b36b34), then paste the token before
  and then click "Authorize"
8. After then close the popup, and here we go, now you can access the product endpoint.


