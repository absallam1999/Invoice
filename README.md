// Create New User
{
  "userName": "absallam",
  "email": "absallam1999@gmail.com",
  "password": "Mm12345!",
  "confirmPassword": "Mm12345!"
}

// Login
{
  "email": "absallam1999@gmail.com",
  "password": "Mm12345!",
  "rememberMe": true
}

Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJkMjYzNWZiMC1jMzNhLTQxMzgtOWIwZS0yM2I4Yzk3OGNiZTIiLCJqdGkiOiI0N2U3ZDhlYS05MGNiLTRkM2QtYjBjNy01NjQzMjhmODU2YzUiLCJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1laWRlbnRpZmllciI6ImQyNjM1ZmIwLWMzM2EtNDEzOC05YjBlLTIzYjhjOTc4Y2JlMiIsImh0dHA6Ly9zY2hlbWFzLnhtbHNvYXAub3JnL3dzLzIwMDUvMDUvaWRlbnRpdHkvY2xhaW1zL25hbWUiOiJhYnNhbGxhbSIsImV4cCI6MTc1NzIwMDY4OCwiaXNzIjoiSW52b2ljZSJ9.5xWRgC1yCUPQf738pJe-djArFZVthGcBZ_ZWte6r9MM

// Create New Category
{
  "name": "New Category",
  "description": "New Category Description"
}

// Create New Client
{
  "name": "Client",
  "email": "client@example.com",
  "phoneNumber": "123456789",
  "address": "Cairo, Egypt"
}

// Update Range Clients
[
  {
      "id": "MF8G3K6Y",
      "name": "New Client",
      "email": "new@client.com",
      "phoneNumber": "12344321",
      "address": "New Cairo"
  },
 {
       "id": "MF8G4CQX",
      "name": "New Client2",
      "email": "new@client2.com",
      "phoneNumber": "12344321",
      "address": "New Cairo"
 }
]

// Create New Invoice
{
  "tax": true,
  "discountType": "Amount",
  "discountValue": 10,
  "finalValue": 90,
  "invoiceStatus": 0,
  "invoiceType": 1,
  "termsConditions": "Terms and Condition.",
  "clientId": "MF8SMRSX",
  "storeId": "MF8U08MJ",
  "languageId": "ar",
  "payments": [],
  "invoiceItems": [
   {
      "quantity": 2,
      "unitPrice": 100,
      "productId": "MF8GBLJH"
    }
  ]
}


// Create New Language
{
  "name": 1,
  "target": 2
}


// Create New Store
{
  "name": "New Store",
  "description": "New Store Description",
  "tax": true,
  "paymentMethods": 1,
  "languageId": "ar",
  "userId": "d2635fb0-c33a-4138-9b0e-23b8c978cbe2"
}

// Modify appSettings.json File:
  "Stripe": {
    "Secret_key": "sk_test_51RqUuWByun3VvENhtD9lsr4ep5elIoe00beRvbVvFGmcaioKCEXckeVoXiiM3k6U9IB9PQKXyh1tuOOjSU6ciMv800GWkjFoqO",
    "Publishable_key": "pk_test_51RqUuWByun3VvENhRxap2Se1EaSVovUfKZhg73ev1COZY86QRBu7HQUbO98ubJwI4aeYNI7F5pIjnRIKAUMGEDDA00icnZPzYP"
  }