using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using System.Windows.Media.Animation;
using System.Threading.Tasks;

namespace CyberSecurityBot
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // Variables to store current user info, quiz progress, and task management

        private string userName = "";
        private string userInterest = "";
        private int currentQuestionIndex = 0;
        private int quizScore = 0;
        private void MainGrid_Loaded(object sender, RoutedEventArgs e)
        {
            // This automatically focuses the chatbot input TextBox when main screen loads
            UserInputTextBox.Focus();
        }

        //Core app data collections 

        private List<string> activityLog = new();     // Keeps log of user actions
        private List<CyberTask> taskList = new();      // Stores user-created tasks
        private Dictionary<string, List<string>> keywordResponses = new();          // Bot replies based on keywords
        private List<QuizQuestion> quizQuestions = new();       // List of quiz questions
        private Random rnd = new Random();        // Used for randomizing responses and questions
        private bool awaitingReminderConfirmation = false;       // Tracks if the user is being asked to set a reminder
        private string lastTopic = "";       // Last topic mentioned by the user (for reminders)


        public MainWindow()
        {
            InitializeComponent();
            PlayGreetingAudio();        // Play welcome sound
            InitializeBotKnowledge();   // Load keyword-based responses    
            InitializeQuizQuestions();  // Load quiz questions
        }

        private void PlayGreetingAudio()
        {
            //creating an instance for the media class
            MediaPlayer Audio_greet = new MediaPlayer();


            //get the path automatical
            string fullPath = AppDomain.CurrentDomain.BaseDirectory;

            //then replace the \\bin\\Debug\\net8.0-windows
            string replaced = fullPath.Replace("\\bin\\Debug\\net8.0-windows", "");

            //combine paths once done replacing
            string combine_path = System.IO.Path.Combine(replaced, "greeting.wav");

            //combine the url as uri
            Audio_greet.Open(new Uri(combine_path, UriKind.Relative));

            //play sound
            Audio_greet.Play();
        }


        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            userName = username.Text.Trim(); // Get the username from the input field

            if (!string.IsNullOrWhiteSpace(userName)) 
            {
                LoginGrid.Visibility = Visibility.Hidden;
                MainGrid.Visibility = Visibility.Visible;
                username.Clear(); // Clear the username input field
               
                // Set the initial focus to the UserInputTextBox after login
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    UserInputTextBox.Focus();
                }), DispatcherPriority.Render);


                LogActivity($"User {userName} logged in.");  
                ChatListBox.Items.Add(new TextBlock
                {
                    Text = $"ChatBot: Welcome {userName}! What cybersecurity topic are you interested in?",
                    Foreground = Brushes.DarkBlue
                });
            }
            else
            {
                MessageBox.Show("Please enter your name.");  // Show message if username is empty
            }
        }

        private void Username_KeyDown(object sender, KeyEventArgs e)
        {   // Keyboard shortcut to add task using Enter
            if (e.Key == Key.Enter) LoginButton_Click(sender, e);  
        }


        // Chatbot Logic 
        private void SendButton_Click(object sender, RoutedEventArgs e)  
        {
            string input = UserInputTextBox.Text.Trim();
            if (string.IsNullOrWhiteSpace(input)) return;

            // Display user message in chat
            ChatListBox.Items.Add(new TextBlock
            {
                Text = $"{userName}: {input}",
                Foreground = Brushes.ForestGreen
            });

            string response;

            // Handles confirmation for reminders
            if (awaitingReminderConfirmation)
            {
                if (input.ToLower().Contains("yes"))
                {
                    response = $"Okay! I've created a reminder task for: {lastTopic}";  
                    // Add a reminder task for the user
                    taskList.Add(new CyberTask
                    {    
                        Title = lastTopic,
                        Description = $"Reminder: {lastTopic}",
                        ReminderDate = DateTime.Now.AddDays(3),
                        IsCompleted = false
                    });
                    RefreshTaskList();
                    LogActivity($"Task created from chat: {lastTopic}");
                }
                else if (input.ToLower().Contains("no"))
                {
                    response = "No worries! Let me know if you change your mind.";
                }
                else
                {
                    response = "Please answer yes or no 😊";
                }

                awaitingReminderConfirmation = false;
            }
            else
            {
                // Task Extraction Trigger 
                if (input.ToLower().Contains("remind"))
                {
                    awaitingReminderConfirmation = true;

                    // Clean up input: remove common phrases like "remind me to"
                    string cleaned = input.ToLower()
                        .Replace("remind me to", "", StringComparison.OrdinalIgnoreCase)
                        .Replace("remind me", "", StringComparison.OrdinalIgnoreCase)
                        .Replace("remind", "", StringComparison.OrdinalIgnoreCase)
                        .Trim();

                    lastTopic = string.IsNullOrWhiteSpace(cleaned) ? input : cleaned;

                    response = $"Would you like to add \"{lastTopic}\" as a task with a reminder?";
                }
                else
                {
                    // Standard chatbot response
                    response = GetBotResponse(input);
                }
            }

            // Display chatbot reply
            ChatListBox.Items.Add(new TextBlock
            {
                Text = $"ChatBot: {response}",
                Foreground = Brushes.DarkBlue
            });

            // Clear input and log
            UserInputTextBox.Clear();
            LogActivity($"User asked: {input} | Bot: {response}");
        }

        private void UserInputTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {  // Keyboard shortcut to add task using Enter
                SendButton_Click(sender, e);
                e.Handled = true;
            }
        }

        private void InitializeBotKnowledge()  
        {
            keywordResponses = new Dictionary<string, List<string>>
            {             // topics and responses of the chatbot knowledge 
                { "password", new List<string> {
                    "Use strong passwords with symbols and numbers.",
                    "Avoid reusing the same password on multiple sites.",
                    "Avoid personal info like birthdates in passwords.",
                    "Use a password manager to stay safe."
                }},
                { "phishing", new List<string> {
                    "Phishing emails try to trick you into clicking fake links.",
                    "Never trust urgent messages from unknown senders.",
                     "Report suspicious emails.",
                     "Don't download attachments from unknown senders.",
                     "Banks never ask for credentials by email."
                }},
                { "privacy", new List<string> {
                    "Review privacy settings on your social media.",
                    "Be careful what info you share publicly.",
                    "Disable location sharing when not needed."
                }},
                { "malware", new List<string> {
                    "Avoid downloading files from sketchy sites.",
                    "Malware can log your keystrokes and steal data.",
                    "Keep your antivirus software up to date."
                }},
                { "remind", new List<string> {
                    "Got it! Would you like to add this as a task with a reminder?",
                    "Sounds like something important. Want me to remind you later?"
                }},
                { "2fa", new List<string> {
                    "Two-Factor Authentication adds extra security to your accounts.",
                    "Enable 2FA to protect against stolen passwords."
                }},
            };
        }

        private string GetBotResponse(string input)
        {
            string lower = input.ToLower();

            // Stores first interest
            if (string.IsNullOrEmpty(userInterest))
            {
                userInterest = lower;
                return $"I'll remember you're interested in {userInterest}. Ask away!";
            }

            // Sentiment detection  
            if (lower.Contains("remind") || lower.Contains("task"))
            {
                awaitingReminderConfirmation = true;
                lastTopic = input;
                return "Would you like to add that as a task with a reminder?";
            }

            if (lower.Contains("how are you"))
                return "I'm great and ready to help you with cybersecurity tips!";

            if (lower.Contains("worried") || lower.Contains("scared"))
                return "It’s okay to feel worried. Let’s take it step by step.";

            if (lower.Contains("frustrated"))
                return "Take a deep breath. Cybersecurity can be tricky, but I’ll help.";

            if (lower.Contains("thanks") || lower.Contains("great"))
                return $"You're welcome! {userName}!";


            // Keyword matches
            foreach (var key in keywordResponses.Keys)
            {
                if (lower.Contains(key))
                {
                    var responses = keywordResponses[key];
                    return responses[rnd.Next(responses.Count)];
                }
            }

            return "Hmm, I didn’t catch that. Try asking about phishing, passwords, privacy, or 2FA!";
        }

        //TASK ASSISTANT
        private void TaskInput_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {  // Keyboard shortcut to add task using Enter
                AddTask_Click(sender, e);
                e.Handled = true;
            }
        }

        // Handles the Task Reminder Date selection and task saving
        private void AddTask_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TaskTitleBox.Text)) { MessageBox.Show("Title required."); return; } // Check if title is empty
            if (TaskReminderPicker.SelectedDate == null) { MessageBox.Show("Select a date."); return; } // Check if date is selected

            int hour = HourPicker.SelectedIndex != -1
                ? int.Parse(((ComboBoxItem)HourPicker.SelectedItem).Content.ToString() ?? "0") // Get hour from ComboBox
                : 0;
            int minute = MinutePicker.SelectedIndex != -1
                ? int.Parse(((ComboBoxItem)MinutePicker.SelectedItem).Content.ToString() ?? "0") // Get minute from ComboBox
                : 0;

            DateTime reminder = TaskReminderPicker.SelectedDate.Value.AddHours(hour).AddMinutes(minute); // Combine date and time

            CyberTask task = new()
            {
                Title = TaskTitleBox.Text.Trim(),
                Description = TaskDescriptionBox.Text.Trim(),
                ReminderDate = reminder,
                IsCompleted = false
            };

            taskList.Add(task);
            RefreshTaskList();
            LogActivity($"Task added: {task.Title}");

            // Reset input fields
            TaskTitleBox.Clear();
            TaskDescriptionBox.Clear();
            TaskReminderPicker.SelectedDate = null;
            HourPicker.SelectedIndex = 0;
            MinutePicker.SelectedIndex = 0;
        }

        // Refreshes the task list display
        private void RefreshTaskList()
        {
            TaskListBox.ItemsSource = null;
            TaskListBox.ItemsSource = taskList;
        }

        // Marks a selected task as completed
        private void CompleteTask_Click(object sender, RoutedEventArgs e)
        {
            if (TaskListBox.SelectedItem is CyberTask selected)
            {
                selected.IsCompleted = true;
                RefreshTaskList();
                LogActivity($"Task marked complete: {selected.Title}");
            }
        }

        // Deletes a selected task from the list
        private void DeleteTask_Click(object sender, RoutedEventArgs e)
        {
            if (TaskListBox.SelectedItem is CyberTask selected)
            {
                taskList.Remove(selected);
                RefreshTaskList();
                LogActivity($"Task deleted: {selected.Title}");
            }
        }


        private void InitializeQuizQuestions()
        {  // Initializes the quiz questions with a mix of cybersecurity topics
            quizQuestions = new List<QuizQuestion>
            {
                new(" What should you do if you get an email asking for your password?",
                    new List<string> { "A.Ignore it", "B.Report it as phishing", "C.Reply with password", "D.Click the link" }, 1),
                new("True or False: Using '123456' is a secure password.",
                    new List<string> { "A.True", "B.False" }, 1),
                new(" Which of the following is a type of malware?",
                    new List<string> { "A.Phishing", "B.Ransomware", "C.VPN", "D.Firewall" }, 1),
                new("How often should you update your passwords?",
                    new List<string> { "A.Every 6–12 months", "B.Never", "C.Only if hacked", "D.Every 10 years" }, 0),
                new("True or False: Public Wi-Fi is safe for banking.",
                    new List<string> { "A.True", "B.False" }, 1),
                new(" What does 2FA stand for?",
                    new List<string> { "A.Two-Factor Authentication", "B.Fast Access", "C.Free App" }, 0),
                new("What should you do before clicking a link in an email?",
                    new List<string> { "A.Hover to preview", "B.Click quickly", "C.Trust it if it says urgent" }, 0),
                new("True or False: Only hackers get hacked.",
                 new List <string> { "A.True", "B.False" }, 1)
            };

            quizQuestions = quizQuestions.OrderBy(q => rnd.Next()).Take(5).ToList();
            LoadQuestion();
        }

        private void LoadQuestion()
        {
            QuizProgressBar.Maximum = quizQuestions.Count;

            if (currentQuestionIndex >= quizQuestions.Count)
            {
                string summary = $"Final Score: {quizScore}/{quizQuestions.Count} ";
                // Provide feedback based on score
                if (quizScore == 5) summary += "🎉 Perfect!";
                else if (quizScore >= 4) summary += "🌟 Great job!";
                else if (quizScore >= 3) summary += "👍 Not bad!";
                else summary += "💡 Keep practicing!";

                QuestionTextBlock.Text = summary;
                ScoreTextBlock.Text = summary;
                RetakeButton.Visibility = Visibility.Visible;
                AnswerListBox.ItemsSource = null;
                return;
            }

            var q = quizQuestions[currentQuestionIndex];
            QuestionTextBlock.Text = q.Text;
            AnswerListBox.ItemsSource = q.Options;
            QuizProgressBar.Value = currentQuestionIndex;
        }

        private void AnimateProgressBar(double from, double to) // Animates the progress bar value change
        {
            var animation = new DoubleAnimation
            {
                From = from,
                To = to,
                Duration = TimeSpan.FromMilliseconds(400),
                EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }
            };

            QuizProgressBar.BeginAnimation(ProgressBar.ValueProperty, animation);
        }

        private void AnswerListBox_KeyDown(object sender, KeyEventArgs e)
        {   // Keyboard shortcut to add task using Enter
            if (e.Key == Key.Enter) NextQuestion_Click(sender, e);
        }

        // Handles the 'Next' button click in the Quiz tab
        private void NextQuestion_Click(object sender, RoutedEventArgs e)
        {
            if (AnswerListBox.SelectedIndex == -1) return;

            int correct = quizQuestions[currentQuestionIndex].CorrectIndex;

            for (int i = 0; i < AnswerListBox.Items.Count; i++)
            {
                var item = (ListBoxItem)AnswerListBox.ItemContainerGenerator.ContainerFromIndex(i);
                if (item != null)
                {
                    item.Background = i == correct ? Brushes.LightGreen :
                        (i == AnswerListBox.SelectedIndex ? Brushes.IndianRed : Brushes.Transparent);
                }
            }

            if (AnswerListBox.SelectedIndex == correct)
            {
                quizScore++;
                LogActivity("Quiz: Correct answer");
            }
            else LogActivity("Quiz: Incorrect answer");

            AnswerListBox.IsEnabled = false;

            Dispatcher.InvokeAsync(async () =>
            {
                await Task.Delay(1500);
                currentQuestionIndex++;
                AnswerListBox.IsEnabled = true;
                LoadQuestion();
            });

            AnimateProgressBar(QuizProgressBar.Value, currentQuestionIndex + 1);
        }

        // Handles the 'Retake Quiz' button click
        private void RetakeButton_Click(object sender, RoutedEventArgs e)
        {
            quizScore = 0;
            currentQuestionIndex = 0;
            RetakeButton.Visibility = Visibility.Collapsed;
            InitializeQuizQuestions();
        }

        //LOG SYSTEM
        private void LogActivity(string action)
        {
            if (activityLog.Count >= 10)
                activityLog.RemoveAt(0);

            activityLog.Add($"{DateTime.Now:HH:mm} - {action}");
            ActivityLogListBox.ItemsSource = null;
            ActivityLogListBox.ItemsSource = activityLog.ToList();
        }

        //QUIZ AND CYBERTASK
        public class CyberTask
        {
            public string? Title { get; set; }
            public string? Description { get; set; }
            public DateTime? ReminderDate { get; set; }
            public bool IsCompleted { get; set; }

            public override string ToString()
            {
                string status = IsCompleted ? "[✔]" : "[ ]";
                string reminder = ReminderDate.HasValue ? $" (Remind: {ReminderDate.Value:yyyy-MM-dd HH:mm})" : "";
                return $"{status} {Title} - {Description}{reminder}";
            }
        }

        public class QuizQuestion
        {
            public string Text { get; }
            public List<string> Options { get; }
            public int CorrectIndex { get; }

            public QuizQuestion(string text, List<string> options, int correctIndex)
            {
                Text = text;
                Options = options;
                CorrectIndex = correctIndex;
            }
        }
    }
}
