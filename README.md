# Cummulative 1 : School Database Management System

This project is a Minimum Viable Product (MVP) for managing the Teachers table in a School Database using ASP.NET Core Web API and MVC. It provides basic CRUD (Create, Read, Update, Delete) functionality for the Teachers table.

## Features

- **Read Functionality**:
  - List all teachers.
  - Display detailed information about a specific teacher.
  - Search for teachers by name, hire date, or salary.

- **API Endpoints**:
  - `GET /api/TeacherData/ListTeachers`: Returns a list of all teachers.

- **Web Pages**:
  - `/TeacherPage/List`: Displays a list of all teachers with a search bar.
  - `/TeacherPage/Show/{id}`: Displays detailed information about a specific teacher.

## Technologies Used

- **ASP.NET Core**: For building the Web API and MVC application.
- **MySQL**: As the database to store teacher information.
- **Entity Framework**: For database access and management.
- **Razor Views**: For server-side rendering of web pages.
