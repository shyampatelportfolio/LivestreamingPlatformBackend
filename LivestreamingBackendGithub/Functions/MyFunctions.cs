using LiveStreamingBackend.Data;
using NewsBackend.Data;
using System.Reflection;
using System.Text.RegularExpressions;

namespace LiveStreamingBackend.Functions
{
    public class MyFunctions
    {
        public static int getWordAmount(string inputText)
        {
            int wordsSum = 0;
            string[] wordsArray = Regex.Split(inputText, @"\s+");
            int numberOfWords = wordsArray.Length;
            wordsSum += numberOfWords;

            return wordsSum;
        }
        public static int getLetterAmount(string inputText)
        {
            int letterSum = 0;
            string lettersOnly = Regex.Replace(inputText, "[^a-zA-Z]", "");
            int numberOfLetters = lettersOnly.Length;
            letterSum += numberOfLetters;

            return letterSum;
        }
        public static int generateRandomInteger(int n)
        {
            var random = new Random();
            int randomNumber = random.Next(0, n);
            return randomNumber;
        }
        public static int generateRandomIntegerScope(int n, int m)
        {
            var random = new Random();
            int randomNumber = random.Next(n, m);
            return randomNumber;
        }

        public static List<int> generateRandomIntegerList(int n, int m)
        {

            List<int> list = new List<int>();

            int randomNumber;

            for (int i = 0; i < m; i++)
            {
                randomNumber = generateRandomInteger(n);
                while (list.Contains(randomNumber))
                {
                    randomNumber = generateRandomInteger(n);
                }
                list.Add(randomNumber);
            }

            return list;
        }

        public static List<int> generateRandomIntegerListExclude(int n, int m, int k)
        {

            List<int> list = new List<int>();

            list.Add(k);

            int randomNumber;

            for (int i = 0; i < m - 1; i++)
            {
                randomNumber = generateRandomInteger(n);
                while (list.Contains(randomNumber))
                {
                    randomNumber = generateRandomInteger(n);
                }
                list.Add(randomNumber);
            }

            return list;
        }



        public static List<int> generateRandomIntegerListExcludeList(int n, int m, List<int> ints)
        {

            List<int> list = new List<int>();


            foreach (int k in ints)
            {
                list.Add(k);
            }

            int randomNumber;

            for (int i = 0; i < m - 1; i++)
            {
                randomNumber = generateRandomInteger(n);
                while (list.Contains(randomNumber))
                {
                    randomNumber = generateRandomInteger(n);
                }
                list.Add(randomNumber);
            }

            return list;
        }


        public static string generateRandomName()
        {
            string[] names = {
                                  "Bubblegum", "Starlight", "Jukebox", "Lunar", "Techno",
                                  "Velvet", "Quasar", "Nebula", "Flamingo", "Tangerine",
                                  "Zephyr", "Plush", "Galactic", "Sonic", "Pixel",
                                  "Cobalt", "Dragonfly", "Stardust", "Moonlight", "Mocha",
                                  "Lullaby", "Quantum", "Firefly", "Prism", "Disco",
                                  "Mango", "Enigma", "Rainbow", "Giggly", "Cinnamon",
                                  "Tidal", "PolkaDot", "Solar", "Cosmic", "Sizzling",
                                  "Nova", "Midnight", "Jazz", "Bumblebee", "Quasar",
                                  "Solaris", "Electro", "Orchid", "Mystic", "Jamboree",
                                  "Nebula", "Bubblegum", "Lullaby", "Velvet", "Neon"
                                };
            string[] surnames = {
                                      "Bard", "Shadow", "Jellybean", "Lullaby", "Tiger",
                                      "Vortex", "Quill", "Nectar", "Fandango", "Tornado",
                                      "Zigzag", "Penguin", "Gazelle", "Seashell", "Pirate",
                                      "Cactus", "Dance", "Sparrow", "Mischief", "Fiesta",
                                      "Jellybean", "Lynx", "Quasar", "Fiesta", "Panther",
                                      "Sloth", "Buzzard", "Viper", "Raccoon", "Noodle",
                                      "Mongoose", "Jackal", "Ballet", "Quokka", "Sphinx",
                                      "Elephant", "Rhythm", "Giraffe", "Turtle", "Tapir",
                                      "Lynx", "Surfer", "Bison", "Llama", "Vampire", "Nightingale",
                                      "Quokka", "Sloth", "Mammoth", "Corgi", "Dolphin",
                                      "Pangolin", "Chameleon", "Lynx", "Mermaid", "Gorilla",
                                      "Vulture", "Emu", "Sphinx", "Bat", "Nudibranch",
                                      "Squirrel", "Giraffe", "Cobra", "Sloth", "Newt",
                                      "Vampire", "Centipede", "Stoat", "Nuthatch"
                                };
            int rand1 = generateRandomInteger(49);
            int rand2 = generateRandomInteger(68);

            string s1 = names[rand1];
            string s2 = surnames[rand2];
            string result = s1 + " " + s2;
            return result;
        }

        public static string generateRandomDate()
        {
            Random random = new Random();

            DateTime startDate = new DateTime(2020, 12, 1);
            DateTime endDate = new DateTime(2023, 12, 31);

            int range = (endDate - startDate).Days;

            int randomDays = random.Next(range);

            DateTime randomDate = startDate.AddDays(randomDays);

            string formattedDate = $"{GetOrdinalDay(randomDate.Day)} {randomDate:MMMM yyyy}";

            return formattedDate;
        }

        public static string GetOrdinalDay(int day)
        {
            if (day >= 11 && day <= 13)
            {
                return $"{day}th";
            }

            switch (day % 10)
            {
                case 1:
                    return $"{day}st";
                case 2:
                    return $"{day}nd";
                case 3:
                    return $"{day}rd";
                default:
                    return $"{day}th";
            }
        }


        public static bool checkMessageSize( string message)
        {
            int letterSum = message.Length;
            Console.WriteLine(letterSum);
            if (letterSum > 400)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public static bool checkMessageSizeAuth(string message)
        {
            int letterSum = message.Length;
            Console.WriteLine(letterSum);
            if (letterSum > 250)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        public static bool checkMessageSizeArray(List<string> messages)
        {
            bool isGoodSize = true;
            messages.ForEach(message =>
            {
                bool isGoodSizeTemp = checkMessageSize(message);
                if(!isGoodSizeTemp)
                {
                    isGoodSize = false;
                }
            });
            return isGoodSize;
        }
        public static bool checkMessageSizeUserRegister(UserDto2 user)
        {
            bool isGoodSize = true;
            bool isGoodSizeTemp1 = checkMessageSizeAuth(user.Email);
            if (!isGoodSizeTemp1)
            {
                isGoodSize = false;
            }
            bool isGoodSizeTemp2 = checkMessageSizeAuth(user.Password);
            if (!isGoodSizeTemp2)
            {
                isGoodSize = false;
            }
            bool isGoodSizeTemp3 = checkMessageSizeAuth(user.Name);
            if (!isGoodSizeTemp3)
            {
                isGoodSize = false;
            }
            return isGoodSize;
        }
        public static bool checkMessageSizeUserLogin(UserDto user)
        {
            bool isGoodSize = true;
            bool isGoodSizeTemp2 = checkMessageSizeAuth(user.Password);
            if (!isGoodSizeTemp2)
            {
                isGoodSize = false;
            }
            bool isGoodSizeTemp3 = checkMessageSizeAuth(user.Name);
            if (!isGoodSizeTemp3)
            {
                isGoodSize = false;
            }
            return isGoodSize;
        }
        public static List<string> trimStringList(List<string> list)
        {
            List<string> listNew = new List<string>();
            list.ForEach(x =>
            {

                string newMessage = Regex.Replace(x, @"\s+", " ");
                listNew.Add(newMessage);
            });
            return listNew;
        }
     
    }
}
