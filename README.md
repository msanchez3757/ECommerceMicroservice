ASP.NET 10 | C# | Clean Architecture | JWT Auth | API Gateway | Rate Limiting | xUnit Testing

This project is a microservice-based e-commerce system built with ASP.NET 10 and C#, structured around Clean Architecture and independent solutions for each API. It demonstrates real-world backend development practices including authentication, API gateway routing, service isolation, and comprehensive unit testing.

Microservices Overview

This system is composed of four independently built and deployable services:

 Authentication API

Provides JWT token generation and role-based authentication

Supports User Registration, Login, and Role Assignment

Issues access tokens used across the entire microservice ecosystem

Product API

Secured endpoints for CRUD operations on products

Only authenticated users with appropriate roles can POST / Create products

Clean layering with Services → Repositories → Controllers

Order API

Responsible for placing and managing orders

Consumes product and user information using clean separation of domain logic

Built following SOLID principles and domain-driven patterns

 API Gateway

Handles all incoming external traffic

Implements:

Rate Limiting

Request Timeout Policies

Centralized Routing

Protects internal services from overload and exposes a unified entry point

🏛️ Architecture Highlights

This project uses a multi-solution structure to maintain strict boundaries between each microservice:

✔ Clean Architecture (Domain → Application → Infrastructure → Presentation)
✔ Dependency Injection in all APIs
✔ Independent databases per service (microservice best practice)
✔ Minimal coupling — each API can be deployed or scaled separately
✔ Consistent folder patterns across solutions

Authentication Flow

Users register in the Auth API

They receive a JWT token after logging in

They include the token in requests through Postman or a client

Authorized actions (e.g., creating products) require elevated roles

Testing

All critical components across the microservices are covered using:

xUnit for structured unit testing

FakeItEasy for mocking interfaces and external dependencies

FluentAssertions for expressive, readable assertions

This ensures reliable behavior, proper dependency handling, and clean, testable service layers.

Technologies Used
Category	Tools
Backend	ASP.NET 10, C#, Clean Architecture
Auth	JWT, ASP.NET Identity
Testing	xUnit, FakeItEasy, FluentAssertions
API Gateway	YARP / Reverse Proxy, Rate Limiting, Timeout Policies
Tools	Visual Studio, Postman, GitHub

How to Run

Clone the repository:

git clone https://github.com/msanchez3757/ECommerceMicroservice.git


Open the solution in Visual Studio 2022

Run each microservice individually (they run on separate ports)

Use Postman to test the endpoints:

Register → Login → Get JWT → Access other APIs

Folder Structure
/ECommerceMicroservice
   /AuthAPI
   /ProductAPI
   /OrderAPI
   /GatewayAPI
   README.md


Each folder is its own Visual Studio solution, representing a completely isolated microservice.

Features Demonstrated

Clean Microservice Design

Secure JWT Auth

Role-Based Authorization

API Gateway with Rate Limiting

Fully Isolated Services

Clean Architecture Patterns

Professional Test Suite

Postman Integration Testing
