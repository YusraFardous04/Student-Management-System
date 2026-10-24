Student Management System is a full-stack web application built with ASP.NET Core Razor Pages and C#, designed to help schools manage students, courses, subjects, grades, and attendance in one place.

This project started as a learning task during my 3-week internship, and grew into a complete system with an AI-powered assistant at its core.

#Features
1. Student Management —> Add and manage students along with their personal info and enrolled courses.
2. KPI Dashboard — >View overall school performance as well as performance broken down by specific class.
3. Subjects & Classes — >Browse the subjects and classes the school teaches. Click into a subject to see grades, and drill down into a specific section to view its students, their marks, and absences.
4. AI Assistant (RAG-powered) —>Ask questions about your data in natural language. The assistant can tell you a student's grades, a teacher's details, or essentially anything about the school's data.

#Tech Stack
Backend: ASP.NET Core Razor Pages, C#
AI Assistant: Retrieval-Augmented Generation (RAG)
Model: Ollama (locally hosted LLM)

#How the AI Assistant Works
The assistant uses a RAG (Retrieval-Augmented Generation) pipeline: instead of relying on the model's general knowledge, it retrieves relevant data from the system and feeds it to the Ollama-hosted model as context. This grounds every answer in the actual student, teacher, and grade data — so responses stay accurate and relevant to the school.


## Setup
1. Create the database: `dotnet ef database update` (in the Api project)
2. Load the sample data: run `seed.sql` against the database
3. Add your Ollama key: `dotnet user-secrets set "Ollama:ApiKey" "<your key>"`
4. Run the Api project, then the Web project
