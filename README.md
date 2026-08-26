# IUBAT-Student-Service-Request-System

A web-based Student Service Request System developed for university administration using **ASP.NET Core 8 MVC**, **Entity Framework Core**, **Supabase PostgreSQL**, and **Bootstrap 5**.

## Overview
The IUBAT Student Service Request System streamlines student applications for academic services (ID Card Replacements, Transcript Requests, and Certificate Requests), providing real-time tracking, administrative review workflows, and role-based access control.

## Key Features
- **Student Portal**:
  - Secure registration and cookie-based authentication.
  - Interactive dashboard with service request submission and status tracking (`Pending`, `Processing`, `Completed`, `Rejected`).
  - View detailed history and administrative remarks.
- **Administrative Staff Portal**:
  - Centralized overview of all incoming student requests.
  - Status updates and official administrative feedback dispatch.
- **Database**:
  - Backed by cloud PostgreSQL (Supabase) via Entity Framework Core (`Npgsql`).
  - Automated schema migration and account seeding.

## Tech Stack
- **Framework**: ASP.NET Core 8 MVC
- **ORM**: Entity Framework Core 8 with Npgsql
- **Database**: Supabase PostgreSQL
- **Frontend**: Razor Views, Bootstrap 5, Bootstrap Icons
- **Security**: Cookie Authentication & Argon2/PBKDF2 Password Hashing
