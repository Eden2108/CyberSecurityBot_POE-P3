#  CyberSecurity Awareness ChatBot (WPF Application)

A comprehensive and interactive desktop application built with C# and WPF to promote cybersecurity awareness through a conversational chatbot, personal task assistant, and quiz game.

---

## 🎯 Purpose

This chatbot is designed to:
- Educate users on common cybersecurity threats (e.g., phishing, 2FA, strong passwords)
- Simulate a conversation with keyword recognition and sentiment analysis
- Encourage responsible behavior through tasks and interactive quizzes
- Track user activity and provide reminders to reinforce safe habits

---

##  Features

###  ChatBot with NLP Simulation
- Remembers user's name and interest topic
- Responds based on detected keywords (e.g., "password", "malware", "privacy")
- Simulates Natural Language Processing using flexible string matching
- Responds to emotional keywords (e.g., "worried", "frustrated")
- Auto-scrolls and supports Enter key to send messages
- Color-coded messages (user vs. bot)

### 📅 Task Assistant with Reminders
- Users can create tasks like “Enable 2FA”
- Each task includes:
  - Title
  - Description
  - Date **and** time reminder using `DatePicker` and `ComboBoxes`
- Tasks can be marked as complete or deleted
- Includes chat-style reminder confirmation (e.g., “Would you like a reminder?” → “Yes”)

###  Cybersecurity Quiz Game
- Includes 8 multiple-choice and True/False questions
- Randomized quiz each time
- Highlights correct and incorrect answers
- Animated progress bar
- Score tracking and final score feedback using emoji/stars
- Retake button to restart the quiz

### 📜 Activity Log
- Records last 10 actions:
  - Added/removed tasks
  - Quiz activity
  - Reminders set
  - User inputs and chatbot responses

### Audio Greeting
- Plays a welcome audio (`greeting.wav`) when the app starts
- Uses `MediaPlayer` and relative path detection

###  Polished WPF UI
- Professional layout using XAML
- Tab-based navigation: ChatBot | Task Assistant | Quiz | Activity Log
- Hover effects and focus management
- Keyboard navigation support (Enter key on login/chat/quiz/task)

---
## 🔧 How to Use

1. Launch the app. A welcome sound will play.
2. Enter your name on the login screen.
3. Navigate between tabs:
   - Use **ChatBot** to ask questions like “What is phishing?”
   - Use **Task Assistant** to create, complete, or delete reminders.
   - Play the **Quiz** to test your cybersecurity knowledge.
4. Activity Log tracks what you do.

---

## 🛠️ Installation / Setup

### Requirements
- Visual Studio 2022+
- .NET 8.0 SDK 
- WPF Desktop App template

### Setup Instructions
1. Clone the repository or download the ZIP.
2. Open the solution in Visual Studio.
3. Add `greeting.wav` to the root of the project:
   - Right-click → Properties:
     - Build Action: `Content`
     - Copy to Output Directory: `Copy if newer`
4. Build and run the project.

---

## 📁 Project Structure


---

## 🏷 Version History

| Tag | Description |
|-----|-------------|
| `v1.0.0` | Base chatbot UI + keyword responses |
| `v2.0.0` | Task Assistant with reminders |
| `v3.0.0` | Quiz feature with progress bar |
| `v3.3.0` | Audio + full interaction polish |
| `v4.0.0` | Final version with complete features and UI enhancements |

---

## Educational Value

This project demonstrates my practical implementation of:
- Event-driven programming in C#
- GUI design using WPF and XAML
- Basic AI logic through keyword and NLP simulation
- Secure UX design patterns for awareness training

---

##  License

This project was created for PROG6221 module.

