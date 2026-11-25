using System;
using System.Collections.Generic;
using System.Linq;

namespace AseanPhonebook
{
    // ===================== CLASS 1: Student =====================
    // Represents ONE student entry in the phonebook.
    public class Student
    {
        public string StudentNumber { get; set; }
        public string Surname { get; set; }
        public string FirstName { get; set; }
        public string Occupation { get; set; }
        public char Gender { get; set; }          // 'M' or 'F'
        public int CountryCode { get; set; }
        public string AreaCode { get; set; }
        public string PhoneNumber { get; set; }

        public Student(string studentNumber, string surname, string firstName,
                       string occupation, char gender,
                       int countryCode, string areaCode, string phoneNumber)
        {
            StudentNumber = studentNumber;
            Surname = surname;
            FirstName = firstName;
            Occupation = occupation;
            Gender = gender;
            CountryCode = countryCode;
            AreaCode = areaCode;
            PhoneNumber = phoneNumber;
        }

        private string Pronoun => Gender == 'M' ? "His" : "Her";

        // Example: 63-2-4567890
        public string FullNumber => $"{CountryCode}-{AreaCode}-{PhoneNumber}";

        // Used in the Edit menu.
        // Example: Sukarno Lee is a Doctor. His number is 63-2-4567890
        public string ToEditString()
        {
            return $"{FirstName} {Surname} is a {Occupation}. {Pronoun} number is {FullNumber}";
        }

        // Used in the Search results.
        // Example: Saint, John, with student number 2000-123, is a doctor. His phone number is 63-2-9998765
        public string ToSearchString()
        {
            return $"{Surname}, {FirstName}, with student number {StudentNumber}, is a {Occupation}. " +
                   $"{Pronoun} phone number is {FullNumber}";
        }
    }

    // ===================== CLASS 2: Phonebook =====================
    // Stores all students and does the work: add, find, search.
    public class Phonebook
    {
        // A List has no size limit, so there is no cap on students.
        private readonly List<Student> students = new List<Student>();

        // The 5 supported countries and their codes.
        public static readonly Dictionary<int, string> Countries = new Dictionary<int, string>
        {
            { 60, "Malaysia" },
            { 62, "Indonesia" },
            { 63, "Philippines" },
            { 65, "Singapore" },
            { 66, "Thailand" }
        };

        public static bool IsValidCountryCode(int code)
        {
            return Countries.ContainsKey(code);
        }

        public bool StudentNumberExists(string studentNumber)
        {
            return FindByStudentNumber(studentNumber) != null;
        }

        public void Add(Student student)
        {
            students.Add(student);
        }

        // Returns the matching student, or null if not found.
        public Student FindByStudentNumber(string studentNumber)
        {
            return students.FirstOrDefault(s =>
                s.StudentNumber.Equals(studentNumber, StringComparison.OrdinalIgnoreCase));
        }

        // Returns students whose country code is in the list, sorted by surname.
        public List<Student> SearchByCountry(List<int> countryCodes)
        {
            return students
                .Where(s => countryCodes.Contains(s.CountryCode))
                .OrderBy(s => s.Surname, StringComparer.OrdinalIgnoreCase)
                .ThenBy(s => s.FirstName, StringComparer.OrdinalIgnoreCase)
                .ToList();
        }
    }

    // ===================== CLASS 3: Program =====================
    // Handles menus and user input. The real work is done by Phonebook.
    public class Program
    {
        private static readonly Phonebook phonebook = new Phonebook();

        public static void Main(string[] args)
        {
            bool running = true;
            while (running)
            {
                Console.WriteLine();
                Console.WriteLine("[1] Store to ASEAN phonebook");
                Console.WriteLine("[2] Edit entry in ASEAN phonebook");
                Console.WriteLine("[3] Search ASEAN phonebook by country");
                Console.WriteLine("[4] Exit");
                Console.Write("Enter choice: ");

                switch (Console.ReadLine()?.Trim())
                {
                    case "1": StoreEntries(); break;
                    case "2": EditEntry(); break;
                    case "3": SearchByCountry(); break;
                    case "4": running = false; break;
                    default: Console.WriteLine("Invalid option. Try again."); break;
                }
            }
        }

        // ---------- [1] STORE ----------
        private static void StoreEntries()
        {
            string again;
            do
            {
                string number;
                while (true)
                {
                    number = ReadText("Enter student number: ");
                    if (!phonebook.StudentNumberExists(number)) break;
                    Console.WriteLine("Error: that student number already exists.");
                }

                string surname = ReadText("Enter surname: ");
                string firstName = ReadText("Enter first name: ");
                string occupation = ReadText("Enter occupation: ");
                char gender = ReadGender();
                int countryCode = ReadCountryCode();
                string areaCode = ReadDigits("Enter area code: ");
                string phone = ReadDigits("Enter number: ");

                phonebook.Add(new Student(number, surname, firstName, occupation,
                                          gender, countryCode, areaCode, phone));
                Console.WriteLine("Entry saved.");

                again = ReadYesNo("Do you want to enter another entry [Y/N]? ");
            } while (again == "Y");
        }

        // ---------- [2] EDIT ----------
        private static void EditEntry()
        {
            string number = ReadText("Enter student number: ");
            Student student = phonebook.FindByStudentNumber(number);

            if (student == null)
            {
                Console.WriteLine("Error: student number not found.");
                return;
            }

            bool editing = true;
            bool firstTime = true;
            while (editing)
            {
                Console.WriteLine(firstTime
                    ? $"Here is the existing information about {student.StudentNumber}:"
                    : $"Here is the updated information about {student.StudentNumber}:");
                Console.WriteLine(student.ToEditString());
                firstTime = false;

                Console.WriteLine("Which of the following information do you wish to change?");
                Console.WriteLine("[1] Student number [2] Surname [3] Gender [4] Occupation");
                Console.WriteLine("[5] Country code [6] Area code [7] Phone number");
                Console.WriteLine("[8] None - Go back to main menu");
                Console.Write("Enter choice: ");

                switch (Console.ReadLine()?.Trim())
                {
                    case "1":
                        while (true)
                        {
                            string newNumber = ReadText("Enter new student number: ");
                            bool taken = phonebook.StudentNumberExists(newNumber)
                                         && !newNumber.Equals(student.StudentNumber, StringComparison.OrdinalIgnoreCase);
                            if (!taken) { student.StudentNumber = newNumber; break; }
                            Console.WriteLine("Error: that student number already exists.");
                        }
                        break;
                    case "2": student.Surname = ReadText("Enter new surname: "); break;
                    case "3": student.Gender = ReadGender(); break;
                    case "4": student.Occupation = ReadText("Enter new occupation: "); break;
                    case "5": student.CountryCode = ReadCountryCode(); break;
                    case "6": student.AreaCode = ReadDigits("Enter new area code: "); break;
                    case "7": student.PhoneNumber = ReadDigits("Enter new number: "); break;
                    case "8": editing = false; break;
                    default: Console.WriteLine("Invalid option. Try again."); break;
                }
            }
        }

        // ---------- [3] SEARCH ----------
        private static void SearchByCountry()
        {
            // menu number -> country code
            var menu = new Dictionary<int, int>
            {
                { 1, 63 }, { 2, 66 }, { 3, 65 }, { 4, 62 }, { 5, 60 }
            };

            var chosen = new List<int>();
            bool all = false;
            int count = 1;

            Console.WriteLine("From which country:");
            Console.WriteLine("[1] Philippines [2] Thailand [3] Singapore [4] Indonesia");
            Console.WriteLine("[5] Malaysia [6] ALL [0] No More");

            while (!all)
            {
                Console.Write($"Enter choice {count}: ");
                string input = Console.ReadLine()?.Trim();

                if (input == "0") break;
                if (input == "6") { all = true; break; }

                if (int.TryParse(input, out int n) && menu.ContainsKey(n))
                {
                    int code = menu[n];
                    if (!chosen.Contains(code)) chosen.Add(code);
                    count++;
                }
                else
                {
                    Console.WriteLine("Invalid choice. Try again.");
                }
            }

            if (all) chosen = Phonebook.Countries.Keys.ToList();

            if (chosen.Count == 0)
            {
                Console.WriteLine("No countries selected.");
                return;
            }

            string names = all
                ? "all countries"
                : string.Join(" and ", chosen.Select(c => Phonebook.Countries[c]));

            List<Student> results = phonebook.SearchByCountry(chosen);

            Console.WriteLine();
            Console.WriteLine($"Here are the students from {names}:");
            if (results.Count == 0)
                Console.WriteLine("(No students found.)");
            foreach (Student s in results)
                Console.WriteLine(s.ToSearchString());
        }

        // ---------- INPUT HELPERS ----------
        private static string ReadText(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);
                string input = Console.ReadLine()?.Trim();
                if (!string.IsNullOrEmpty(input)) return input;
                Console.WriteLine("This cannot be empty.");
            }
        }

        private static string ReadDigits(string prompt)
        {
            while (true)
            {
                string input = ReadText(prompt);
                if (input.All(char.IsDigit)) return input;
                Console.WriteLine("Digits only, please.");
            }
        }

        private static char ReadGender()
        {
            while (true)
            {
                string input = ReadText("Enter gender (M for male, F for female): ").ToUpper();
                if (input == "M" || input == "F") return input[0];
                Console.WriteLine("Please enter M or F.");
            }
        }

        private static int ReadCountryCode()
        {
            while (true)
            {
                string input = ReadText("Enter country code: ");
                if (int.TryParse(input, out int code) && Phonebook.IsValidCountryCode(code))
                    return code;
                Console.WriteLine("Invalid country code. Valid: 60, 62, 63, 65, 66.");
            }
        }

        private static string ReadYesNo(string prompt)
        {
            while (true)
            {
                string input = ReadText(prompt).ToUpper();
                if (input == "Y" || input == "N") return input;
                Console.WriteLine("Please enter Y or N.");
            }
        }
    }
}