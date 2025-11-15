//using System;
//using Microsoft.EntityFrameworkCore.Migrations;

//#nullable disable

//#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

//namespace invoice.Migrations
//{
//    /// <inheritdoc />
//    public partial class newMig : Migration
//    {
//        /// <inheritdoc />
//        protected override void Up(MigrationBuilder migrationBuilder)
//        {
//            migrationBuilder.CreateTable(
//                name: "AspNetRoles",
//                columns: table => new
//                {
//                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
//                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
//                    NormalizedName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
//                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true)
//                },
//                constraints: table =>
//                {
//                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
//                });

//            migrationBuilder.CreateTable(
//                name: "AspNetUsers",
//                columns: table => new
//                {
//                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
//                    PaypalEmail = table.Column<string>(type: "nvarchar(max)", nullable: true),
//                    StripeAccountId = table.Column<string>(type: "nvarchar(max)", nullable: true),
//                    EdfaAccountId = table.Column<string>(type: "nvarchar(max)", nullable: true),
//                    TabAccountId = table.Column<string>(type: "nvarchar(max)", nullable: true),
//                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
//                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
//                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
//                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
//                    UserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
//                    NormalizedUserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
//                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
//                    NormalizedEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
//                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
//                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
//                    SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
//                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
//                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
//                    PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
//                    TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
//                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
//                    LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
//                    AccessFailedCount = table.Column<int>(type: "int", nullable: false)
//                },
//                constraints: table =>
//                {
//                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
//                });

//            migrationBuilder.CreateTable(
//                name: "Languages",
//                columns: table => new
//                {
//                    Id = table.Column<string>(type: "nvarchar(8)", maxLength: 8, nullable: false),
//                    Name = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
//                    Target = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
//                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
//                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
//                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
//                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
//                },
//                constraints: table =>
//                {
//                    table.PrimaryKey("PK_Languages", x => x.Id);
//                });

//            migrationBuilder.CreateTable(
//                name: "PaymentMethods",
//                columns: table => new
//                {
//                    Id = table.Column<string>(type: "nvarchar(8)", maxLength: 8, nullable: false),
//                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
//                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
//                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
//                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
//                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
//                },
//                constraints: table =>
//                {
//                    table.PrimaryKey("PK_PaymentMethods", x => x.Id);
//                });

//            migrationBuilder.CreateTable(
//                name: "AspNetRoleClaims",
//                columns: table => new
//                {
//                    Id = table.Column<int>(type: "int", nullable: false)
//                        .Annotation("SqlServer:Identity", "1, 1"),
//                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false),
//                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
//                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
//                },
//                constraints: table =>
//                {
//                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
//                    table.ForeignKey(
//                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
//                        column: x => x.RoleId,
//                        principalTable: "AspNetRoles",
//                        principalColumn: "Id",
//                        onDelete: ReferentialAction.Cascade);
//                });

//            migrationBuilder.CreateTable(
//                name: "AspNetUserClaims",
//                columns: table => new
//                {
//                    Id = table.Column<int>(type: "int", nullable: false)
//                        .Annotation("SqlServer:Identity", "1, 1"),
//                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
//                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
//                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
//                },
//                constraints: table =>
//                {
//                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
//                    table.ForeignKey(
//                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
//                        column: x => x.UserId,
//                        principalTable: "AspNetUsers",
//                        principalColumn: "Id",
//                        onDelete: ReferentialAction.Cascade);
//                });

//            migrationBuilder.CreateTable(
//                name: "AspNetUserLogins",
//                columns: table => new
//                {
//                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
//                    ProviderKey = table.Column<string>(type: "nvarchar(450)", nullable: false),
//                    ProviderDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
//                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
//                },
//                constraints: table =>
//                {
//                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
//                    table.ForeignKey(
//                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
//                        column: x => x.UserId,
//                        principalTable: "AspNetUsers",
//                        principalColumn: "Id",
//                        onDelete: ReferentialAction.Cascade);
//                });

//            migrationBuilder.CreateTable(
//                name: "AspNetUserRoles",
//                columns: table => new
//                {
//                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
//                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false)
//                },
//                constraints: table =>
//                {
//                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
//                    table.ForeignKey(
//                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
//                        column: x => x.RoleId,
//                        principalTable: "AspNetRoles",
//                        principalColumn: "Id",
//                        onDelete: ReferentialAction.Cascade);
//                    table.ForeignKey(
//                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
//                        column: x => x.UserId,
//                        principalTable: "AspNetUsers",
//                        principalColumn: "Id",
//                        onDelete: ReferentialAction.Cascade);
//                });

//            migrationBuilder.CreateTable(
//                name: "AspNetUserTokens",
//                columns: table => new
//                {
//                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
//                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
//                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
//                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
//                },
//                constraints: table =>
//                {
//                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
//                    table.ForeignKey(
//                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
//                        column: x => x.UserId,
//                        principalTable: "AspNetUsers",
//                        principalColumn: "Id",
//                        onDelete: ReferentialAction.Cascade);
//                });

//            migrationBuilder.CreateTable(
//                name: "Categories",
//                columns: table => new
//                {
//                    Id = table.Column<string>(type: "nvarchar(8)", maxLength: 8, nullable: false),
//                    Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
//                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
//                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
//                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
//                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
//                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
//                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
//                },
//                constraints: table =>
//                {
//                    table.PrimaryKey("PK_Categories", x => x.Id);
//                    table.ForeignKey(
//                        name: "FK_Categories_AspNetUsers_UserId",
//                        column: x => x.UserId,
//                        principalTable: "AspNetUsers",
//                        principalColumn: "Id",
//                        onDelete: ReferentialAction.Cascade);
//                });

//            migrationBuilder.CreateTable(
//                name: "Clients",
//                columns: table => new
//                {
//                    Id = table.Column<string>(type: "nvarchar(8)", maxLength: 8, nullable: false),
//                    Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
//                    Email = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
//                    PhoneNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
//                    Address = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
//                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
//                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
//                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
//                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
//                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
//                },
//                constraints: table =>
//                {
//                    table.PrimaryKey("PK_Clients", x => x.Id);
//                    table.ForeignKey(
//                        name: "FK_Clients_AspNetUsers_UserId",
//                        column: x => x.UserId,
//                        principalTable: "AspNetUsers",
//                        principalColumn: "Id",
//                        onDelete: ReferentialAction.Cascade);
//                });

//            migrationBuilder.CreateTable(
//                name: "Currencies",
//                columns: table => new
//                {
//                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
//                    CurrencyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
//                    Country = table.Column<string>(type: "nvarchar(max)", nullable: false),
//                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
//                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
//                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
//                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
//                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
//                },
//                constraints: table =>
//                {
//                    table.PrimaryKey("PK_Currencies", x => x.Id);
//                    table.ForeignKey(
//                        name: "FK_Currencies_AspNetUsers_UserId",
//                        column: x => x.UserId,
//                        principalTable: "AspNetUsers",
//                        principalColumn: "Id",
//                        onDelete: ReferentialAction.Cascade);
//                });

//            migrationBuilder.CreateTable(
//                name: "Notifications",
//                columns: table => new
//                {
//                    Id = table.Column<string>(type: "nvarchar(8)", maxLength: 8, nullable: false),
//                    Title = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
//                    Message = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
//                    Type = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
//                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
//                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
//                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
//                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
//                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
//                },
//                constraints: table =>
//                {
//                    table.PrimaryKey("PK_Notifications", x => x.Id);
//                    table.ForeignKey(
//                        name: "FK_Notifications_AspNetUsers_UserId",
//                        column: x => x.UserId,
//                        principalTable: "AspNetUsers",
//                        principalColumn: "Id",
//                        onDelete: ReferentialAction.Cascade);
//                });

//            migrationBuilder.CreateTable(
//                name: "PaymentLinks",
//                columns: table => new
//                {
//                    Id = table.Column<string>(type: "nvarchar(8)", maxLength: 8, nullable: false),
//                    Value = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
//                    Slug = table.Column<string>(type: "nvarchar(max)", nullable: false),
//                    IsActivated = table.Column<bool>(type: "bit", nullable: false),
//                    PaymentsNumber = table.Column<int>(type: "int", maxLength: 50, nullable: false),
//                    ExpireDate = table.Column<DateTime>(type: "datetime2", nullable: true),
//                    MaxPaymentsNumber = table.Column<int>(type: "int", nullable: true),
//                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
//                    Currency = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
//                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
//                    Cash = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
//                    BankTransfer = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
//                    PayPal = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
//                    PaymentOptions_Tax = table.Column<bool>(type: "bit", nullable: false),
//                    ClientName = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
//                    ClientEmail = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
//                    ClientPhone = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
//                    ClientAddress = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
//                    TermsAndConditions = table.Column<string>(type: "nvarchar(max)", nullable: true),
//                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
//                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
//                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
//                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
//                },
//                constraints: table =>
//                {
//                    table.PrimaryKey("PK_PaymentLinks", x => x.Id);
//                    table.ForeignKey(
//                        name: "FK_PaymentLinks_AspNetUsers_UserId",
//                        column: x => x.UserId,
//                        principalTable: "AspNetUsers",
//                        principalColumn: "Id",
//                        onDelete: ReferentialAction.Cascade);
//                });

//            migrationBuilder.CreateTable(
//                name: "Stores",
//                columns: table => new
//                {
//                    Id = table.Column<string>(type: "nvarchar(8)", maxLength: 8, nullable: false),
//                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
//                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
//                    Slug = table.Column<string>(type: "nvarchar(max)", nullable: false),
//                    IsActivated = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
//                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
//                    Shipping_FromStore = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
//                    Shipping_Delivery = table.Column<bool>(type: "bit", nullable: false),
//                    Shipping_Region = table.Column<string>(type: "nvarchar(max)", nullable: true),
//                    Shipping_DeliveryFee = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
//                    StoreSettings_Logo = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
//                    StoreSettings_CoverImage = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
//                    StoreSettings_Color = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, defaultValue: "#FFFFFF"),
//                    StoreSettings_Currency = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false, defaultValue: "USD"),
//                    StoreSettings_Country = table.Column<string>(type: "nvarchar(max)", nullable: false),
//                    StoreSettings_purchaseOptions_Name = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
//                    StoreSettings_purchaseOptions_Email = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
//                    StoreSettings_purchaseOptions_phone = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
//                    StoreSettings_purchaseOptions_Address = table.Column<bool>(type: "bit", nullable: false),
//                    StoreSettings_purchaseOptions_TermsAndConditions = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
//                    ContactInformations_Phone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
//                    ContactInformations_Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
//                    ContactInformations_Location = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
//                    ContactInformations_Facebook = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
//                    ContactInformations_WhatsApp = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
//                    ContactInformations_Instagram = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
//                    PaymentOptions_Cash = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
//                    PaymentOptions_BankTransfer = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
//                    PaymentOptions_PayPal = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
//                    PaymentOptions_Tax = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
//                    ParallelMergeOptions = table.Column<int>(type: "int", nullable: false),
//                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
//                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
//                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
//                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
//                },
//                constraints: table =>
//                {
//                    table.PrimaryKey("PK_Stores", x => x.Id);
//                    table.ForeignKey(
//                        name: "FK_Stores_AspNetUsers_UserId",
//                        column: x => x.UserId,
//                        principalTable: "AspNetUsers",
//                        principalColumn: "Id",
//                        onDelete: ReferentialAction.Restrict);
//                });

//            migrationBuilder.CreateTable(
//                name: "Taxes",
//                columns: table => new
//                {
//                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
//                    TaxNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
//                    TaxName = table.Column<string>(type: "nvarchar(max)", nullable: false),
//                    Value = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
//                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
//                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
//                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
//                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
//                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
//                },
//                constraints: table =>
//                {
//                    table.PrimaryKey("PK_Taxes", x => x.Id);
//                    table.ForeignKey(
//                        name: "FK_Taxes_AspNetUsers_UserId",
//                        column: x => x.UserId,
//                        principalTable: "AspNetUsers",
//                        principalColumn: "Id",
//                        onDelete: ReferentialAction.Cascade);
//                });

//            migrationBuilder.CreateTable(
//                name: "Pages",
//                columns: table => new
//                {
//                    Id = table.Column<string>(type: "nvarchar(8)", maxLength: 8, nullable: false),
//                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
//                    Content = table.Column<string>(type: "nvarchar(max)", nullable: false),
//                    Image = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
//                    InFooter = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
//                    InHeader = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
//                    StoreId = table.Column<string>(type: "nvarchar(8)", nullable: false),
//                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
//                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
//                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
//                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
//                },
//                constraints: table =>
//                {
//                    table.PrimaryKey("PK_Pages", x => x.Id);
//                    table.ForeignKey(
//                        name: "FK_Pages_Stores_StoreId",
//                        column: x => x.StoreId,
//                        principalTable: "Stores",
//                        principalColumn: "Id",
//                        onDelete: ReferentialAction.Cascade);
//                });

//            migrationBuilder.CreateTable(
//                name: "Products",
//                columns: table => new
//                {
//                    Id = table.Column<string>(type: "nvarchar(8)", maxLength: 8, nullable: false),
//                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
//                    MainImage = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
//                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
//                    Quantity = table.Column<int>(type: "int", nullable: true, defaultValue: 0),
//                    InProductList = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
//                    InPOS = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
//                    InStore = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
//                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
//                    CategoryId = table.Column<string>(type: "nvarchar(8)", nullable: true),
//                    StoreId = table.Column<string>(type: "nvarchar(8)", nullable: true),
//                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
//                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
//                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
//                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
//                },
//                constraints: table =>
//                {
//                    table.PrimaryKey("PK_Products", x => x.Id);
//                    table.ForeignKey(
//                        name: "FK_Products_AspNetUsers_UserId",
//                        column: x => x.UserId,
//                        principalTable: "AspNetUsers",
//                        principalColumn: "Id",
//                        onDelete: ReferentialAction.Restrict);
//                    table.ForeignKey(
//                        name: "FK_Products_Categories_CategoryId",
//                        column: x => x.CategoryId,
//                        principalTable: "Categories",
//                        principalColumn: "Id",
//                        onDelete: ReferentialAction.SetNull);
//                    table.ForeignKey(
//                        name: "FK_Products_Stores_StoreId",
//                        column: x => x.StoreId,
//                        principalTable: "Stores",
//                        principalColumn: "Id");
//                });

//            migrationBuilder.CreateTable(
//                name: "Invoices",
//                columns: table => new
//                {
//                    Id = table.Column<string>(type: "nvarchar(8)", maxLength: 8, nullable: false),
//                    Code = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
//                    Value = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
//                    Tax = table.Column<bool>(type: "bit", nullable: false),
//                    DiscountType = table.Column<int>(type: "int", nullable: true),
//                    DiscountValue = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
//                    FinalValue = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
//                    InvoiceStatus = table.Column<int>(type: "int", nullable: false),
//                    InvoiceType = table.Column<int>(type: "int", nullable: false),
//                    ClientId = table.Column<string>(type: "nvarchar(8)", nullable: true),
//                    LanguageId = table.Column<string>(type: "nvarchar(8)", nullable: false),
//                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
//                    TermsConditions = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
//                    TaxId = table.Column<string>(type: "nvarchar(450)", nullable: true),
//                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
//                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
//                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
//                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
//                },
//                constraints: table =>
//                {
//                    table.PrimaryKey("PK_Invoices", x => x.Id);
//                    table.ForeignKey(
//                        name: "FK_Invoices_AspNetUsers_UserId",
//                        column: x => x.UserId,
//                        principalTable: "AspNetUsers",
//                        principalColumn: "Id",
//                        onDelete: ReferentialAction.Restrict);
//                    table.ForeignKey(
//                        name: "FK_Invoices_Clients_ClientId",
//                        column: x => x.ClientId,
//                        principalTable: "Clients",
//                        principalColumn: "Id",
//                        onDelete: ReferentialAction.SetNull);
//                    table.ForeignKey(
//                        name: "FK_Invoices_Languages_LanguageId",
//                        column: x => x.LanguageId,
//                        principalTable: "Languages",
//                        principalColumn: "Id",
//                        onDelete: ReferentialAction.Restrict);
//                    table.ForeignKey(
//                        name: "FK_Invoices_Taxes_TaxId",
//                        column: x => x.TaxId,
//                        principalTable: "Taxes",
//                        principalColumn: "Id");
//                });

//            migrationBuilder.CreateTable(
//                name: "InvoiceItems",
//                columns: table => new
//                {
//                    Id = table.Column<string>(type: "nvarchar(8)", maxLength: 8, nullable: false),
//                    Quantity = table.Column<int>(type: "int", nullable: false),
//                    UnitPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
//                    LineTotal = table.Column<decimal>(type: "decimal(18,2)", nullable: false, computedColumnSql: "[Quantity] * [UnitPrice]"),
//                    InvoiceId = table.Column<string>(type: "nvarchar(8)", nullable: false),
//                    ProductId = table.Column<string>(type: "nvarchar(8)", nullable: false),
//                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
//                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
//                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
//                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
//                },
//                constraints: table =>
//                {
//                    table.PrimaryKey("PK_InvoiceItems", x => x.Id);
//                    table.ForeignKey(
//                        name: "FK_InvoiceItems_Invoices_InvoiceId",
//                        column: x => x.InvoiceId,
//                        principalTable: "Invoices",
//                        principalColumn: "Id",
//                        onDelete: ReferentialAction.Cascade);
//                    table.ForeignKey(
//                        name: "FK_InvoiceItems_Products_ProductId",
//                        column: x => x.ProductId,
//                        principalTable: "Products",
//                        principalColumn: "Id",
//                        onDelete: ReferentialAction.Cascade);
//                });

//            migrationBuilder.CreateTable(
//                name: "Orders",
//                columns: table => new
//                {
//                    Id = table.Column<string>(type: "nvarchar(8)", maxLength: 8, nullable: false),
//                    IsPaid = table.Column<bool>(type: "bit", nullable: false),
//                    ShippingMethod = table.Column<int>(type: "int", nullable: false),
//                    PaymentType = table.Column<int>(type: "int", nullable: false),
//                    OrderStatus = table.Column<int>(type: "int", nullable: true),
//                    DeliveryCost = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
//                    InvoiceId = table.Column<string>(type: "nvarchar(8)", nullable: false),
//                    StoreId = table.Column<string>(type: "nvarchar(8)", nullable: false),
//                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
//                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
//                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
//                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
//                },
//                constraints: table =>
//                {
//                    table.PrimaryKey("PK_Orders", x => x.Id);
//                    table.ForeignKey(
//                        name: "FK_Orders_Invoices_InvoiceId",
//                        column: x => x.InvoiceId,
//                        principalTable: "Invoices",
//                        principalColumn: "Id",
//                        onDelete: ReferentialAction.Cascade);
//                    table.ForeignKey(
//                        name: "FK_Orders_Stores_StoreId",
//                        column: x => x.StoreId,
//                        principalTable: "Stores",
//                        principalColumn: "Id",
//                        onDelete: ReferentialAction.Cascade);
//                });

//            migrationBuilder.CreateTable(
//                name: "PayInvoices",
//                columns: table => new
//                {
//                    Id = table.Column<string>(type: "nvarchar(8)", maxLength: 8, nullable: false),
//                    PaidAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
//                    PaymentMethodId = table.Column<string>(type: "nvarchar(8)", nullable: false),
//                    InvoiceId = table.Column<string>(type: "nvarchar(8)", nullable: false),
//                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
//                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
//                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
//                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
//                },
//                constraints: table =>
//                {
//                    table.PrimaryKey("PK_PayInvoices", x => x.Id);
//                    table.ForeignKey(
//                        name: "FK_PayInvoices_Invoices_InvoiceId",
//                        column: x => x.InvoiceId,
//                        principalTable: "Invoices",
//                        principalColumn: "Id",
//                        onDelete: ReferentialAction.Cascade);
//                    table.ForeignKey(
//                        name: "FK_PayInvoices_PaymentMethods_PaymentMethodId",
//                        column: x => x.PaymentMethodId,
//                        principalTable: "PaymentMethods",
//                        principalColumn: "Id",
//                        onDelete: ReferentialAction.Restrict);
//                });

//            migrationBuilder.CreateTable(
//                name: "PaymentLinkPayments",
//                columns: table => new
//                {
//                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
//                    PaymentsNumber = table.Column<int>(type: "int", nullable: false),
//                    PaymentType = table.Column<int>(type: "int", nullable: false),
//                    PaymentLinkId = table.Column<string>(type: "nvarchar(8)", nullable: false),
//                    InvoiceId = table.Column<string>(type: "nvarchar(8)", nullable: false),
//                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
//                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
//                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
//                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
//                },
//                constraints: table =>
//                {
//                    table.PrimaryKey("PK_PaymentLinkPayments", x => x.Id);
//                    table.ForeignKey(
//                        name: "FK_PaymentLinkPayments_Invoices_InvoiceId",
//                        column: x => x.InvoiceId,
//                        principalTable: "Invoices",
//                        principalColumn: "Id",
//                        onDelete: ReferentialAction.Cascade);
//                    table.ForeignKey(
//                        name: "FK_PaymentLinkPayments_PaymentLinks_PaymentLinkId",
//                        column: x => x.PaymentLinkId,
//                        principalTable: "PaymentLinks",
//                        principalColumn: "Id",
//                        onDelete: ReferentialAction.Cascade);
//                });

//            migrationBuilder.CreateTable(
//                name: "Payments",
//                columns: table => new
//                {
//                    Id = table.Column<string>(type: "nvarchar(8)", maxLength: 8, nullable: false),
//                    Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
//                    Link = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
//                    Cost = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
//                    Currency = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
//                    GatewaySessionId = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
//                    Type = table.Column<int>(type: "int", nullable: false),
//                    Status = table.Column<int>(type: "int", nullable: false),
//                    ExpiresAt = table.Column<DateTime>(type: "datetime2", nullable: true, defaultValueSql: "DATEADD(DAY, 3, GETUTCDATE())"),
//                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
//                    InvoiceId = table.Column<string>(type: "nvarchar(8)", nullable: false),
//                    PaymentMethodId = table.Column<string>(type: "nvarchar(8)", nullable: false),
//                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
//                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
//                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
//                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
//                },
//                constraints: table =>
//                {
//                    table.PrimaryKey("PK_Payments", x => x.Id);
//                    table.ForeignKey(
//                        name: "FK_Payments_AspNetUsers_UserId",
//                        column: x => x.UserId,
//                        principalTable: "AspNetUsers",
//                        principalColumn: "Id",
//                        onDelete: ReferentialAction.Restrict);
//                    table.ForeignKey(
//                        name: "FK_Payments_Invoices_InvoiceId",
//                        column: x => x.InvoiceId,
//                        principalTable: "Invoices",
//                        principalColumn: "Id",
//                        onDelete: ReferentialAction.Restrict);
//                    table.ForeignKey(
//                        name: "FK_Payments_PaymentMethods_PaymentMethodId",
//                        column: x => x.PaymentMethodId,
//                        principalTable: "PaymentMethods",
//                        principalColumn: "Id",
//                        onDelete: ReferentialAction.Restrict);
//                });

//            migrationBuilder.CreateTable(
//                name: "OrderItems",
//                columns: table => new
//                {
//                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
//                    OrderId = table.Column<string>(type: "nvarchar(8)", nullable: false),
//                    ProductId = table.Column<string>(type: "nvarchar(8)", nullable: false),
//                    Quantity = table.Column<int>(type: "int", nullable: false, defaultValue: 1),
//                    UnitPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
//                    LineTotal = table.Column<decimal>(type: "decimal(18,2)", nullable: false, computedColumnSql: "[Quantity] * [UnitPrice]", stored: true),
//                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
//                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
//                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
//                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
//                },
//                constraints: table =>
//                {
//                    table.PrimaryKey("PK_OrderItems", x => x.Id);
//                    table.ForeignKey(
//                        name: "FK_OrderItems_Orders_OrderId",
//                        column: x => x.OrderId,
//                        principalTable: "Orders",
//                        principalColumn: "Id",
//                        onDelete: ReferentialAction.Cascade);
//                    table.ForeignKey(
//                        name: "FK_OrderItems_Products_ProductId",
//                        column: x => x.ProductId,
//                        principalTable: "Products",
//                        principalColumn: "Id",
//                        onDelete: ReferentialAction.Restrict);
//                });

//            migrationBuilder.InsertData(
//                table: "Languages",
//                columns: new[] { "Id", "CreatedAt", "DeletedAt", "Name", "Target", "UpdatedAt" },
//                values: new object[,]
//                {
//                    { "ar", new DateTime(2025, 10, 27, 23, 34, 26, 827, DateTimeKind.Unspecified).AddTicks(6958), null, "Arabic", "Page", null },
//                    { "en", new DateTime(2025, 10, 27, 23, 34, 26, 827, DateTimeKind.Unspecified).AddTicks(7029), null, "English", "Page", null }
//                });

//            migrationBuilder.InsertData(
//                table: "PaymentMethods",
//                columns: new[] { "Id", "CreatedAt", "DeletedAt", "Name", "UpdatedAt" },
//                values: new object[,]
//                {
//                    { "ap", new DateTime(2025, 10, 27, 23, 34, 26, 827, DateTimeKind.Unspecified).AddTicks(7642), null, "ApplePay", null },
//                    { "bt", new DateTime(2025, 10, 27, 23, 34, 26, 827, DateTimeKind.Unspecified).AddTicks(7528), null, "BankTransfer", null },
//                    { "ca", new DateTime(2025, 10, 27, 23, 34, 26, 827, DateTimeKind.Unspecified).AddTicks(7466), null, "Cash", null },
//                    { "cc", new DateTime(2025, 10, 27, 23, 34, 26, 827, DateTimeKind.Unspecified).AddTicks(7503), null, "CreditCard", null },
//                    { "dc", new DateTime(2025, 10, 27, 23, 34, 26, 827, DateTimeKind.Unspecified).AddTicks(7517), null, "DebitCard", null },
//                    { "dl", new DateTime(2025, 10, 27, 23, 34, 26, 827, DateTimeKind.Unspecified).AddTicks(7710), null, "Delivery", null },
//                    { "gp", new DateTime(2025, 10, 27, 23, 34, 26, 827, DateTimeKind.Unspecified).AddTicks(7655), null, "GooglePay", null },
//                    { "ma", new DateTime(2025, 10, 27, 23, 34, 26, 827, DateTimeKind.Unspecified).AddTicks(7667), null, "Mada", null },
//                    { "pp", new DateTime(2025, 10, 27, 23, 34, 26, 827, DateTimeKind.Unspecified).AddTicks(7540), null, "PayPal", null },
//                    { "sa", new DateTime(2025, 10, 27, 23, 34, 26, 827, DateTimeKind.Unspecified).AddTicks(7691), null, "Sadad", null },
//                    { "sp", new DateTime(2025, 10, 27, 23, 34, 26, 827, DateTimeKind.Unspecified).AddTicks(7680), null, "STCPay", null },
//                    { "st", new DateTime(2025, 10, 27, 23, 34, 26, 827, DateTimeKind.Unspecified).AddTicks(7608), null, "Stripe", null },
//                    { "tp", new DateTime(2025, 10, 27, 23, 34, 26, 827, DateTimeKind.Unspecified).AddTicks(7624), null, "TabPayments", null }
//                });

//            migrationBuilder.CreateIndex(
//                name: "IX_AspNetRoleClaims_RoleId",
//                table: "AspNetRoleClaims",
//                column: "RoleId");

//            migrationBuilder.CreateIndex(
//                name: "RoleNameIndex",
//                table: "AspNetRoles",
//                column: "NormalizedName",
//                unique: true,
//                filter: "[NormalizedName] IS NOT NULL");

//            migrationBuilder.CreateIndex(
//                name: "IX_AspNetUserClaims_UserId",
//                table: "AspNetUserClaims",
//                column: "UserId");

//            migrationBuilder.CreateIndex(
//                name: "IX_AspNetUserLogins_UserId",
//                table: "AspNetUserLogins",
//                column: "UserId");

//            migrationBuilder.CreateIndex(
//                name: "IX_AspNetUserRoles_RoleId",
//                table: "AspNetUserRoles",
//                column: "RoleId");

//            migrationBuilder.CreateIndex(
//                name: "EmailIndex",
//                table: "AspNetUsers",
//                column: "NormalizedEmail");

//            migrationBuilder.CreateIndex(
//                name: "UserNameIndex",
//                table: "AspNetUsers",
//                column: "NormalizedUserName",
//                unique: true,
//                filter: "[NormalizedUserName] IS NOT NULL");

//            migrationBuilder.CreateIndex(
//                name: "IX_Categories_UserId",
//                table: "Categories",
//                column: "UserId");

//            migrationBuilder.CreateIndex(
//                name: "IX_Clients_Email",
//                table: "Clients",
//                column: "Email",
//                unique: true,
//                filter: "[Email] IS NOT NULL");

//            migrationBuilder.CreateIndex(
//                name: "IX_Clients_PhoneNumber",
//                table: "Clients",
//                column: "PhoneNumber",
//                unique: true,
//                filter: "[PhoneNumber] IS NOT NULL");

//            migrationBuilder.CreateIndex(
//                name: "IX_Clients_UserId",
//                table: "Clients",
//                column: "UserId");

//            migrationBuilder.CreateIndex(
//                name: "IX_Currencies_UserId",
//                table: "Currencies",
//                column: "UserId",
//                unique: true);

//            migrationBuilder.CreateIndex(
//                name: "IX_InvoiceItems_InvoiceId",
//                table: "InvoiceItems",
//                column: "InvoiceId");

//            migrationBuilder.CreateIndex(
//                name: "IX_InvoiceItems_ProductId",
//                table: "InvoiceItems",
//                column: "ProductId");

//            migrationBuilder.CreateIndex(
//                name: "IX_Invoices_ClientId",
//                table: "Invoices",
//                column: "ClientId");

//            migrationBuilder.CreateIndex(
//                name: "IX_Invoices_LanguageId",
//                table: "Invoices",
//                column: "LanguageId");

//            migrationBuilder.CreateIndex(
//                name: "IX_Invoices_TaxId",
//                table: "Invoices",
//                column: "TaxId");

//            migrationBuilder.CreateIndex(
//                name: "IX_Invoices_UserId",
//                table: "Invoices",
//                column: "UserId");

//            migrationBuilder.CreateIndex(
//                name: "IX_Languages_Name_Target",
//                table: "Languages",
//                columns: new[] { "Name", "Target" },
//                unique: true);

//            migrationBuilder.CreateIndex(
//                name: "IX_Notifications_UserId",
//                table: "Notifications",
//                column: "UserId");

//            migrationBuilder.CreateIndex(
//                name: "IX_OrderItems_OrderId",
//                table: "OrderItems",
//                column: "OrderId");

//            migrationBuilder.CreateIndex(
//                name: "IX_OrderItems_ProductId",
//                table: "OrderItems",
//                column: "ProductId");

//            migrationBuilder.CreateIndex(
//                name: "IX_Orders_InvoiceId",
//                table: "Orders",
//                column: "InvoiceId",
//                unique: true);

//            migrationBuilder.CreateIndex(
//                name: "IX_Orders_StoreId",
//                table: "Orders",
//                column: "StoreId");

//            migrationBuilder.CreateIndex(
//                name: "IX_Pages_StoreId",
//                table: "Pages",
//                column: "StoreId");

//            migrationBuilder.CreateIndex(
//                name: "IX_PayInvoices_InvoiceId",
//                table: "PayInvoices",
//                column: "InvoiceId",
//                unique: true);

//            migrationBuilder.CreateIndex(
//                name: "IX_PayInvoices_PaymentMethodId",
//                table: "PayInvoices",
//                column: "PaymentMethodId");

//            migrationBuilder.CreateIndex(
//                name: "IX_PaymentLinkPayments_InvoiceId",
//                table: "PaymentLinkPayments",
//                column: "InvoiceId",
//                unique: true);

//            migrationBuilder.CreateIndex(
//                name: "IX_PaymentLinkPayments_PaymentLinkId",
//                table: "PaymentLinkPayments",
//                column: "PaymentLinkId");

//            migrationBuilder.CreateIndex(
//                name: "IX_PaymentLinks_UserId",
//                table: "PaymentLinks",
//                column: "UserId");

//            migrationBuilder.CreateIndex(
//                name: "IX_Payments_GatewaySessionId",
//                table: "Payments",
//                column: "GatewaySessionId",
//                unique: true);

//            migrationBuilder.CreateIndex(
//                name: "IX_Payments_InvoiceId",
//                table: "Payments",
//                column: "InvoiceId");

//            migrationBuilder.CreateIndex(
//                name: "IX_Payments_PaymentMethodId",
//                table: "Payments",
//                column: "PaymentMethodId");

//            migrationBuilder.CreateIndex(
//                name: "IX_Payments_UserId",
//                table: "Payments",
//                column: "UserId");

//            migrationBuilder.CreateIndex(
//                name: "IX_Products_CategoryId",
//                table: "Products",
//                column: "CategoryId");

//            migrationBuilder.CreateIndex(
//                name: "IX_Products_StoreId",
//                table: "Products",
//                column: "StoreId");

//            migrationBuilder.CreateIndex(
//                name: "IX_Products_UserId",
//                table: "Products",
//                column: "UserId");

//            migrationBuilder.CreateIndex(
//                name: "IX_Stores_UserId",
//                table: "Stores",
//                column: "UserId",
//                unique: true);

//            migrationBuilder.CreateIndex(
//                name: "IX_Taxes_UserId",
//                table: "Taxes",
//                column: "UserId",
//                unique: true);
//        }

//        /// <inheritdoc />
//        protected override void Down(MigrationBuilder migrationBuilder)
//        {
//            migrationBuilder.DropTable(
//                name: "AspNetRoleClaims");

//            migrationBuilder.DropTable(
//                name: "AspNetUserClaims");

//            migrationBuilder.DropTable(
//                name: "AspNetUserLogins");

//            migrationBuilder.DropTable(
//                name: "AspNetUserRoles");

//            migrationBuilder.DropTable(
//                name: "AspNetUserTokens");

//            migrationBuilder.DropTable(
//                name: "Currencies");

//            migrationBuilder.DropTable(
//                name: "InvoiceItems");

//            migrationBuilder.DropTable(
//                name: "Notifications");

//            migrationBuilder.DropTable(
//                name: "OrderItems");

//            migrationBuilder.DropTable(
//                name: "Pages");

//            migrationBuilder.DropTable(
//                name: "PayInvoices");

//            migrationBuilder.DropTable(
//                name: "PaymentLinkPayments");

//            migrationBuilder.DropTable(
//                name: "Payments");

//            migrationBuilder.DropTable(
//                name: "AspNetRoles");

//            migrationBuilder.DropTable(
//                name: "Orders");

//            migrationBuilder.DropTable(
//                name: "Products");

//            migrationBuilder.DropTable(
//                name: "PaymentLinks");

//            migrationBuilder.DropTable(
//                name: "PaymentMethods");

//            migrationBuilder.DropTable(
//                name: "Invoices");

//            migrationBuilder.DropTable(
//                name: "Categories");

//            migrationBuilder.DropTable(
//                name: "Stores");

//            migrationBuilder.DropTable(
//                name: "Clients");

//            migrationBuilder.DropTable(
//                name: "Languages");

//            migrationBuilder.DropTable(
//                name: "Taxes");

//            migrationBuilder.DropTable(
//                name: "AspNetUsers");
//        }
//    }
//}
