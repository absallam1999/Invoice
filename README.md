// Create New User
{
  "userName": "absallam",
  "email": "absallam1999@test.com",
  "phoneNumber": "0123456789",
  "password": "Mm12345!",
  "confirmPassword": "Mm12345!"
}


// Login
{
  "email": "absallam1999@test.com",
  "password": "Mm12345!",
  "rememberMe": true
}


Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJjZTlmZjg2MS0zZDQ1LTQ1NjgtYjRmNy03NjNhNjBmODI4ZGIiLCJqdGkiOiJhZTMwN2Q5OC04MTEyLTQ2MzItOWViMi01YzQ1OTFjZDU5NWEiLCJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1laWRlbnRpZmllciI6ImNlOWZmODYxLTNkNDUtNDU2OC1iNGY3LTc2M2E2MGY4MjhkYiIsImh0dHA6Ly9zY2hlbWFzLnhtbHNvYXAub3JnL3dzLzIwMDUvMDUvaWRlbnRpdHkvY2xhaW1zL25hbWUiOiJhYnNhbGxhbSIsImV4cCI6MTc1OTc5NjQ1MywiaXNzIjoiSW52b2ljZSIsImF1ZCI6Ikludm9pY2VfYXVkaWVuY2UifQ.gHekB6Lza7lV783TI_0wMDovQvcrQwkvc8NWMaDWZSc


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
  "invoiceType": "Online",
  "termsConditions": "Invoice Terms and Conditions",
  "clientId": "MG5E8OVL",
  "languageId": "ar",
  "invoiceItems": [
    {
      "quantity": 2,
      "productId": "MG5EEDRA"
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


// Payment Gateways Secret Key [appsettings.json]
    "Stripe": {
    "SecretKey": "sk_test_51RqUuWByun3VvENhtD9lsr4ep5elIoe00beRvbVvFGmcaioKCEXckeVoXiiM3k6U9IB9PQKXyh1tuOOjSU6ciMv800GWkjFoqO",
    "PublishableKey": "pk_test_51RqUuWByun3VvENhRxap2Se1EaSVovUfKZhg73ev1COZY86QRBu7HQUbO98ubJwI4aeYNI7F5pIjnRIKAUMGEDDA00icnZPzYP",
    "WebhookSecret": "whsec_StXeiXoDiJ074yRXjlA98bSeS9CKH4j9"
  },
  "PayPal": {
    "ClientId": "Adu3eQ5Ko772BsijUDjU8HBdA5TP5RJnbSN3MjP0WxihYK4K13Nf99vdhuKsxaQKJLFjwylrpLPP5aJP",
    "ClientSecret": "EAKk42Zbd5ehKLib1a0atI3qlrFbsCkiqUQnwIFqB8GJ0FqgIramplNmSLDGubdYpv6RfMg6aWp_s-eM"
  },