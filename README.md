# Classroom Reservation System

A role-based classroom scheduling system built using **.NET Blazor** and **SQL Server**. Designed for academic institutions to manage courses, instructors, classrooms, and scheduled lectures - all while avoiding conflicts and maintaining a clear timetable.

---

## Features

- 👤 Admin-only user registration (Admins and Faculty).
- 🏫 Classroom management with capacity and department tracking.
- 📘 Course creation and assignment to instructors.
- 📅 Lecture scheduling in fixed 80-minute time slots (8:00 AM – 4:00 PM).
- ⛔ Conflict detection to prevent overlapping lectures.
- 🔒 Role-based UI (Admins vs Faculty).
- 💾 Session persistence using `sessionStorage`.
- 📄 Auto-generated timetable based on scheduled classes.

---

## Tech Stack

- **Frontend:** Blazor WebAssembly 
- **Backend:** ASP.NET Core (.NET 7+)
- **Database:** SQL Server
- **Data Access:** ADO.NET (`Microsoft.Data.SqlClient`)
- **State Management:** Scoped services + `sessionStorage`

---


## 📌 Project Notes

> Faculty room reservation is currently not implemented. Only class scheduling by admin is functional in this version.  
> Passwords are stored in plain text for academic purposes — hashing should be added for production use.
> Make sure the database schema matches the class models used in the project for seamless integration. 

---

## 📷 Screenshots
![image](https://github.com/user-attachments/assets/98ee2454-c1ae-49d4-a27a-438b5254abde)
![image](https://github.com/user-attachments/assets/34a22167-a53b-4188-9627-f2381a5b70b4)
![image](https://github.com/user-attachments/assets/f699c7b1-3688-435e-9029-a768cba8df76)
![image](https://github.com/user-attachments/assets/59dcc658-d64c-49fe-8e85-aa46965a95e1)


