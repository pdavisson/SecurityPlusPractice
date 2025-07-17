# SecurityPlusPractice

**SecurityPlusPractice** is a .NET 8 MVC web application that dynamically builds and seeds a SQL Server database with 10,000 security-related practice questions. It delivers a quiz-based experience to help users test and reinforce their knowledge of security topics — ideal for CompTIA Security+ and similar certifications.

---

## ✨ Features

- ✅ Automatic database creation and seeding on first run
- 🧠 Question sessions pulled randomly from a 10,000-question pool
- 🔄 Session tracking for user, time, and score
- ⚡ Immediate answer feedback with correctness and explanation
- 📊 Score summary on quiz completion
- 🎯 Fully MVC-based architecture with strong separation of concerns
- 🔐 Secure session state using TempData and strongly typed models

---

## 📁 Project Structure

SecurityPlusPractice/
│
├── Controllers/
│ ├── HomeController.cs
│ └── SessionController.cs
│
├── Models/
│ ├── Question.cs
│ ├── Choice.cs
│ ├── Answer.cs
│ ├── Explanation.cs
│ ├── Session.cs
│ └── SessionAnswer.cs
│
├── ViewModels/
│ ├── QuestionViewModel.cs
│ ├── AnswerReviewViewModel.cs
│ ├── ResultViewModel.cs
│ └── SessionSummaryViewModel.cs
│
├── Views/
│ └── Session/
│ ├── Start.cshtml
│ ├── Question.cshtml
│ ├── Review.cshtml
│ └── Result.cshtml
│
├── Data/
│ ├── AppDbContext.cs
│ └── SeedFiles/
│ ├── questions_10k.txt
│ ├── choices_10k.txt
│ ├── answers_10k.txt
│ └── explanations_10k.txt
│
├── Services/
│ └── Seeder.cs
│
├── appsettings.json
├── Program.cs
└── README.md

yaml
Copy
Edit

---

## 🧰 Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [SQL Server LocalDB or SQL Server Express](https://learn.microsoft.com/en-us/sql/database-engine/configure-windows/sql-server-express-localdb)
- Visual Studio 2022 or later
- OS: Windows recommended

---

## 🚀 Getting Started

### 1. Clone the repository

```bash
git clone https://github.com/your-username/SecurityPlusPractice.git
cd SecurityPlusPractice
2. Set up your connection string
Update appsettings.json with your SQL Server instance:

json
Copy
Edit
"ConnectionStrings": {
  "DefaultConnection": "Server=localhost;Database=SecurityPlusPracticeDb;Trusted_Connection=True;TrustServerCertificate=True;"
}
If using SQL Express:
"Server=localhost\\SQLEXPRESS;..."

3. Build and run the app
bash
Copy
Edit
dotnet build
dotnet run
On first run:

The database and schema will be created.

The 10,000-question seed data will be imported from SeedFiles.

🧪 How It Works
Session Flow
Start – User begins session (/Session/Start)

Begin – Random questions are chosen from database (/Session/Begin)

Answering – Each question displays with multiple choices (/Session/Question)

Review – After each submission, feedback is shown with explanation

Finish – Final results are tallied and displayed (/Session/Finish)

Data Flow
TempData holds SessionID and the shuffled question ID list

Answers are logged in SessionAnswers with correctness flag

Final score and percent are calculated in the controller

📦 Seed File Format
Each .txt file in /Data/SeedFiles/ must be tab-delimited (\t) and include a header row. Format:

questions_10k.txt
python-repl
Copy
Edit
ID\tQuestionText
1\tWhat is the purpose of a firewall?
...
choices_10k.txt
bash
Copy
Edit
ID\tChoiceA\tChoiceB\tChoiceC\tChoiceD
1\tTo block traffic\tTo enable DNS\tTo log usage\tTo encrypt data
...
answers_10k.txt
python-repl
Copy
Edit
ID\tCorrectAnswer
1\tA
...
explanations_10k.txt
python-repl
Copy
Edit
ID\tExplanationText
1\tA firewall blocks unauthorized traffic entering or leaving the network.
...
🔍 Known Issues
All questions are assumed to have one correct answer (A–D).

Current session data is not persisted across browser sessions.

Multi-answer questions are not supported.

✅ Future Enhancements
Add user authentication for persistent profiles

Track history and performance analytics

Enable category-based question filtering

Support exporting session results to PDF or CSV

📜 License
This project is licensed under the MIT License. See LICENSE.md for details.

🙌 Credits
Developed by Peter Davisson
© 2025 SecurityPlusPractice – All rights reserved.
