# Invoice API – ASP.NET Web API (with Tap Payments Integration)

A modern, production-ready **ASP.NET Web API** built for managing invoices, clients, payments, stores, and orders, with full integration to:

- **Tap Payments (Saudi Arabia)**
- **Stripe**
- **PayPal**

The API includes secure JWT authentication, multilingual invoice support, online/offline payment flows, and e-commerce style order management.

---

## 📌 Features

### ✔ Authentication & Authorization  
- Register new users  
- Login with JWT  
- Secure endpoints with Bearer tokens  
- Token expiration and refresh support  

### ✔ Invoice System  
- Create Clients  
- Create Stores  
- Create Invoice Items  
- Create Invoices with tax, discount & multi-language support  

### ✔ Payment Processing  
Supports the following gateways:

| Gateway | Country | Supported | Notes |
|--------|----------|-----------|-------|
| **Tap Payments (TAP / تاب البيمنت)** | Saudi Arabia | ✔ | Full API integration |
| Stripe | Global | ✔ | Useful for USD/Testing |
| PayPal | Global | ✔ | Supports client & secret authentication |

---

## 🔐 Authentication Examples

### **Create New User**
{
  "userName": "absallam",
  "PhoneNumber": "0123456789",
  "email": "absallam1999@test.com",
  "password": "Mm12345!",
  "confirmPassword": "Mm12345!"
}

### **Autenticate**
{
  "email": "absallam1999@test.com",
  "password": "Mm12345!",
  "rememberMe": true
}

---

### 📂 API Examples

### **Create New Category**
{
  "name": "New Category",
  "description": "New Category Description"
}

### **Create New Client**
{
  "name": "Client",
  "email": "client@example.com",
  "phoneNumber": "123456789",
  "address": "Cairo, Egypt"
}

### **Create New Invoice**
{
  "tax": true,
  "discountType": "Amount",
  "discountValue": 0,
  "finalValue": 0,
  "invoiceType": "online",
  "termsConditions": "Invoice Terms",
  "clientId": "MH95KSCM",           // Replace with acual Client ID
  "languageId": "AR",
  "invoiceItems": [
    {
      "quantity": 2,
      "productId": "MH95KWB0"   // Replace with acual Product ID
    }
  ]
}

### **Create New Store**
{
  "name": "New Store",
  "description": "New Store Description",
  "tax": true,
  "paymentMethod": "cash",
  "languageId": "AR_S",
  "userId": "03347c41-69e9-437e-8963-69d7ab904153"      // Replace with acual User ID
}

### **Create New Order**
{
  "currency": "USD",
  "storeId": "MFR3I22N",                // Replace with acual Store ID
  "clientId": "MFR3FGW9",                // Replace with acual Client ID
  "languageId": "AR_I",
  "tax": true,
  "discountType": "Amount",
  "discountValue": 0,
  "orderItems": [
    {
      "productId": "MFQWAVOA",                // Replace with acual Product ID
      "quantity": 2
    }
  ]
}

### **Create New Payment (Tap Payments)**
{
  "name": "New Payment",
  "cost": 400.00,
  "currency": "SAR",
  "maxUsageCount": 1,
  "invoiceId": "MH96IUN1",                                      // Replace with acual Invoice ID
  "description": "New Paymet Description",
  "clientId": "MH95KSCM",                                       // Replace with acual Client ID
  "clientEmail": "client@example.com",                         // Replace with acual Client Email
  "paymentMethodId": "tp",
  "metadata": {
    "invoice_code": "INV-638971688487484078"                   // Replace with acual Invoice Code
  }
}