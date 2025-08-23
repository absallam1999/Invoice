// Create New User and Login
{
  "email": "absallam1999@gmail.com",
  "password": "Mm1234",
  "rememberMe": true
}

Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiI1MDMyMzhlOS02OTBiLTRmMDUtOGU0MS1mYzAzYzIwOTUxNTciLCJqdGkiOiJmMTMzMDY0OS1kYTVkLTQ5YzYtYWY5MC04MDIzNTJlNzdjNTQiLCJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1laWRlbnRpZmllciI6IjUwMzIzOGU5LTY5MGItNGYwNS04ZTQxLWZjMDNjMjA5NTE1NyIsImh0dHA6Ly9zY2hlbWFzLnhtbHNvYXAub3JnL3dzLzIwMDUvMDUvaWRlbnRpdHkvY2xhaW1zL25hbWUiOiJhYnNhbGxhbTE5OTkiLCJleHAiOjE3NTU5OTE4OTQsImlzcyI6Ikludm9pY2UifQ.F3FiwOhab7N0ywj62c9-knD8955w56elB9av7C4qIFY

// Create New Category
{
  "name": "New Category2",
  "description": "New Category Description2"
}

{
  "code": "INV-12JU5F23",
  "taxNumber": "1234",
  "value": 100,
  "totalPaid": 20,
  "remainingAmount": 80,
  "discountType": "Amount",
  "discountValue": 10,
  "finalValue": 90,
  "invoiceStatus": 0,
  "invoiceType": 1,
  "termsConditions": "Terms and Condition.",
  "clientId": "MEOR90EV",	// Create New Client
  "storeId": "MEOUJCH6",	// Create New Store
  "languageId": "MEOTKLJC",	// Create New Language
  "payments": [],
  "invoiceItems": [
    {
      "quantity": 2,
      "unitPrice": 1.00,
      "productId": "MEOUM00V"	// Create New Product
    }
  ]
}


// Create New Language
{
  "name": 1,
  "target": 2
}


// Create New Product
{
  "name": "New Product",
  "image": "string",
  "price": 50,
  "quantity": 10,
  "inProductList": true,
  "inPOS": true,
  "inStore": true,
  "userId": "503238e9-690b-4f05-8e41-fc03c2095157",
  "categoryId": "MEOPM65J",
  "storeId": "string",
  "url": "newURL-PRO-1943i4"
}

// Create New Store
{
  "name": "New Store",
  "description": "New Store Description",
  "tax": true,
  "paymentMethods": 1,
  "languageId": "MEOTU53R",
  "userId": "503238e9-690b-4f05-8e41-fc03c2095157"
}

// Modify appSettings.json File:
  "Stripe": {
    "Secret_key": "sk_test_51RqUuWByun3VvENhtD9lsr4ep5elIoe00beRvbVvFGmcaioKCEXckeVoXiiM3k6U9IB9PQKXyh1tuOOjSU6ciMv800GWkjFoqO",
    "Publishable_key": "pk_test_51RqUuWByun3VvENhRxap2Se1EaSVovUfKZhg73ev1COZY86QRBu7HQUbO98ubJwI4aeYNI7F5pIjnRIKAUMGEDDA00icnZPzYP"
  }