// Create New User
{
  "userName": "absallam",
  "PhoneNumber": "0123456789",
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


// Token
Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiI4ZDdlNzdiNi0yMTcyLTQ3YzItOTFkNy0yNTAwZTJiZjdjZjIiLCJqdGkiOiI0OGJkNmU3Mi1hNDEwLTRkNWItODgxZi1hZjNkMGQ3ZTEwNGQiLCJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1laWRlbnRpZmllciI6IjhkN2U3N2I2LTIxNzItNDdjMi05MWQ3LTI1MDBlMmJmN2NmMiIsImh0dHA6Ly9zY2hlbWFzLnhtbHNvYXAub3JnL3dzLzIwMDUvMDUvaWRlbnRpdHkvY2xhaW1zL25hbWUiOiJhYnNhbGxhbSIsImV4cCI6MTc2MjIwMzg0MywiaXNzIjoiSW52b2ljZSIsImF1ZCI6Ikludm9pY2VfYXVkaWVuY2UifQ.xWsu-9zkWcrKYLaNITTbDwMtqoRqgglaSHTvcvpo7Bk


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
  "discountType": "Amount",
  "discountValue": 0,
  "finalValue": 0,
  "invoiceType": "online",
  "termsConditions": "Invoice Terms",
  "clientId": "MH95KSCM",
  "languageId": "AR",
  "invoiceItems": [
    {
      "quantity": 2,
      "productId": "MH95KWB0"
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
  "cost": 400.00,
  "currency": "SAR",
  "maxUsageCount": 1,
  "invoiceId": "MH96IUN1",
  "description": "New Paymet Description",
  "clientId": "MH95KSCM",
  "clientEmail": "client@example.com",
  "paymentMethodId": "tp",
  "metadata": {
    "invoice_code": "INV-638971688487484078"
  }
}


// appsettings.json
  "Stripe": {
    "SecretKey": "sk_test_51RqUuWByun3VvENhtD9lsr4ep5elIoe00beRvbVvFGmcaioKCEXckeVoXiiM3k6U9IB9PQKXyh1tuOOjSU6ciMv800GWkjFoqO",
    "PublishableKey": "pk_test_51RqUuWByun3VvENhRxap2Se1EaSVovUfKZhg73ev1COZY86QRBu7HQUbO98ubJwI4aeYNI7F5pIjnRIKAUMGEDDA00icnZPzYP",
    "WebhookSecret": "whsec_StXeiXoDiJ074yRXjlA98bSeS9CKH4j9"
  },
  "PayPal": {
    "ClientId": "Adu3eQ5Ko772BsijUDjU8HBdA5TP5RJnbSN3MjP0WxihYK4K13Nf99vdhuKsxaQKJLFjwylrpLPP5aJP",
    "ClientSecret": "EAKk42Zbd5ehKLib1a0atI3qlrFbsCkiqUQnwIFqB8GJ0FqgIramplNmSLDGubdYpv6RfMg6aWp_s-eM"
  },
  "TabPayments": {
    "SecretKey": "sk_test_XOq1I7t8TvcKV6QFEZGgf9nm",
    "PublishableKey": "pk_test_qeLuXzx0c4sZIYDgrbUWtO8P",
    "MerchantId": "67989550",
    "WebhookSecret": "...",
    "BaseUrl": "https://api.tap.company",
    "TestMode": true,
    "TimeoutSeconds": 30,
    "RetryCount": 3
  }