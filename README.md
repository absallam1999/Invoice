// Create New User
{
  "userName": "absallam",
  "email": "absallam1999@test.com",
  "password": "Mm12345!",
  "confirmPassword": "Mm12345!"
}

// Login
{
  "email": "absallam1999@test.com",
  "password": "Mm12345!",
  "rememberMe": true
}

Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJlZDZkMGIzMi1hMjFkLTQ5YmYtOTZkYi0xNTRiODM5ZGM4YTMiLCJqdGkiOiI5ZTM4Y2Q5Mi03YmExLTQ2ZWItOTIyMi0yYjYwM2QyZGNiOGIiLCJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1laWRlbnRpZmllciI6ImVkNmQwYjMyLWEyMWQtNDliZi05NmRiLTE1NGI4MzlkYzhhMyIsImh0dHA6Ly9zY2hlbWFzLnhtbHNvYXAub3JnL3dzLzIwMDUvMDUvaWRlbnRpdHkvY2xhaW1zL25hbWUiOiJhYnNhbGxhbSIsImV4cCI6MTc1NzgwMzY1NCwiaXNzIjoiSW52b2ljZSJ9.-QFRxESgM0CvDgSLGQCkJ4SWOe_ZhGPYUmSdCqA0USY

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
  "currency": "USD",
  "discountType": "Amount",
  "discountValue": 0,
  "finalValue": 100,
  "invoiceType": "Active",
  "termsConditions": "Invoice Terms",
  "storeId": "MFITZQER",
  "clientId": "MFITWLEN",
  "languageId": "ar_i",
  "invoiceItems": [
    {
      "quantity": 2,
      "productId": "MFIU266S"
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
  "languageId": "AR_S",
  "userId": "d1e6bb24-d51a-4831-8c73-7d70a23c8ff7"
}

// Modify appSettings.json File:
  "Stripe": {
    "Secret_key": "sk_test_51RqUuWByun3VvENhtD9lsr4ep5elIoe00beRvbVvFGmcaioKCEXckeVoXiiM3k6U9IB9PQKXyh1tuOOjSU6ciMv800GWkjFoqO",
    "Publishable_key": "pk_test_51RqUuWByun3VvENhRxap2Se1EaSVovUfKZhg73ev1COZY86QRBu7HQUbO98ubJwI4aeYNI7F5pIjnRIKAUMGEDDA00icnZPzYP"
  }