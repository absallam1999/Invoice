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


// Create New Invoice
{
  "tax": true,
  "currency": "USD",
  "discountType": "Amount",
  "discountValue": 0,
  "invoiceType": "online",
  "termsConditions": "Invoice Terms",
  "storeId": "MFR3I22N",
  "clientId": "MFR3FGW9",
  "languageId": "AR_I",
  "invoiceItems": [
    {
      "quantity": 1,
      "productId": "MFR3K62C"
    }
  ]
}


// Create New Store
{
  "name": "New Store",
  "description": "New Store Description",
  "tax": true,
  "paymentMethod": "cash",
  "languageId": "AR_S",
  "userId": "03347c41-69e9-437e-8963-69d7ab904153"
}


// Create New Order
{
  "currency": "USD",
  "storeId": "MFR3I22N",
  "clientId": "MFR3FGW9",
  "languageId": "AR_I",
  "tax": true,
  "discountType": "Amount",
  "discountValue": 0,
  "orderItems": [
    {
      "productId": "MFQWAVOA",
      "quantity": 2
    }
  ]
}


// Create New Payment
{
  "name": "New Payment",
  "currency": "USD",
  "cost": 200,
  "maxUsageCount": 1,
  "invoiceId": "MFRB4W25",
  "description": "New Payment Description",
  "clientId": "MFR3FGW9",
  "clientEmail": "client@example.com",
  "paymentMethodId": "st",
  "metadata": {
    "invoice_code": "INV-638939114876833275"
  }
}


// Strip Secret Key [appsettings.json]
  "Stripe": {
    "SecretKey": "sk_test_51RqUuWByun3VvENhtD9lsr4ep5elIoe00beRvbVvFGmcaioKCEXckeVoXiiM3k6U9IB9PQKXyh1tuOOjSU6ciMv800GWkjFoqO",
    "PublishableKey": "pk_test_51RqUuWByun3VvENhRxap2Se1EaSVovUfKZhg73ev1COZY86QRBu7HQUbO98ubJwI4aeYNI7F5pIjnRIKAUMGEDDA00icnZPzYP",
    "WebhookSecret": "whsec_StXeiXoDiJ074yRXjlA98bSeS9CKH4j9"
  }