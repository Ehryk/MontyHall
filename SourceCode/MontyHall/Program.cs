using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MontyHall
{
    class Program
    {
        static bool Text;
        static bool Visual;
        static bool Help;

        static void Main(string[] args)
        {
            Random r = new Random();
            int number = 10;
            int doors = 3;
            int picked = 1;

            Visual = false;
            Help = false;

            if (args.Length == 0)
                Help = true;
            else if (args.Any(a => a.Equals("?")))
                Help = true;
            else if (args.Any(a => a.EqualsIgnoreCase("help")))
                Help = true;
            else if (args.Any(a => a.EqualsIgnoreCase("usage")))
                Help = true;

            if (Help)
            {
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.WriteLine("Usage: MontyHall <strategy> <plays> <doors> <picked>");
                Console.WriteLine("Strategies: Stay, Switch, Random, Alternate");
                return;
            }

            if (args.Any(a => a.EqualsIgnoreCase("visual") || a.EqualsIgnoreCase("v") || a.EqualsIgnoreCase("vt") || a.EqualsIgnoreCase("tv")))
                Visual = true;
            if (args.Any(a => a.EqualsIgnoreCase("text") || a.EqualsIgnoreCase("t") || a.EqualsIgnoreCase("vt") || a.EqualsIgnoreCase("tv")))
                Text = true;

            if (args.Length > 1)
                number = Int32.Parse(args[1]);

            if (args.Length > 2)
                doors = Int32.Parse(args[2]);

            if (args.Length > 3)
                picked = Int32.Parse(args[3]);

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine(" === Monty Hall v{0}.{1} ===", Extensions.Version.Major, Extensions.Version.Minor);

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Playing Monty Hall {0} times with {1} doors, always picking door {2}", number, doors, picked);
            Console.WriteLine("Strategy: {0}", args[0].ToUpper());

            Console.ForegroundColor = ConsoleColor.White;
            if (args[0].ToLower() == "switch")
            {
                PlayRepeat(r, Strategy.Switch, number, doors, picked);
            }
            else if (args[0].ToLower() == "stay")
            {
                PlayRepeat(r, Strategy.Stay, number, doors, picked);
            }
            else if (args[0].ToLower() == "random")
            {
                PlayRepeat(r, Strategy.Random, number, doors, picked);
            }
            else if (args[0].ToLower() == "alternate")
            {
                PlayRepeat(r, Strategy.Alternate, number, doors, picked);
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Specify a Strategy: Switch, Stay, Random, or Alternate");
            }

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("Press any key to continue ...");

            Console.ResetColor();
            Console.ReadKey();
        }

        static void PlayRepeat(Random r, Strategy pStrategy, int pTimes, int pDoors, int pPicked)
        {
            int wins = 0;

            for (int n = 0; n < pTimes; n++)
            {
                if (pStrategy == Strategy.Alternate)
                {
                    Strategy s = (Strategy)((n % 2) + 1);
                    if (PlayMonty(r, s, pDoors, pPicked))
                        wins++;
                }
                else
                {
                    if (PlayMonty(r, pStrategy, pDoors, pPicked))
                        wins++;
                }
            }

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("{0} Results: {1} wins, {2} plays, {3:N2}%", pStrategy.ToString().ToUpper(), wins, pTimes, Decimal.Divide(wins, pTimes) * 100);
        }

        static bool PlayMonty(Random pRandom, Strategy pStrategy, int pDoors = 3, int pPicked = 1)
        {
            //Winning Door
            int door = pRandom.Next(0, pDoors);

            //Initial Pick
            int picked = pPicked - 1;

            //Single Remaining Door after all other non-winning doors are opened
            //Default: the winning door (if you haven't chosen it)
            int remaining = door;

            //If you have chosen the winning door: the random non-picked door (all doors are losing, doesn't matter which one remains)'
            if (picked == door)
            {
                //Pick a non-winning door at random
                while (remaining == door)
                    remaining = pRandom.Next(0, pDoors);
            }

            if (pStrategy == Strategy.Random)
            {
                int newStrategy = pRandom.Next(1, 3);
                pStrategy = (Strategy)newStrategy;
            }

            int result;
            switch (pStrategy)
            {
                case Strategy.Stay:
                    result = picked;
                    if (Text)
                        Console.Write("Winning Door: {0}, Initial: {1}, Remaining: {2}, Picked: {3}, Strategy: Stay,   Result: {4}   ", door + 1, picked + 1, remaining + 1, result + 1, door == result ? "Won " : "Lost");
                    if (Visual)
                        Console.Write(Visualized(pDoors, door, result, picked, remaining));
                    if (Text || Visual)
                        Console.WriteLine();
                    return door == result;

                case Strategy.Switch:
                    result = remaining;
                    if (Text)
                        Console.Write("Winning Door: {0}, Initial: {1}, Remaining: {2}, Picked: {3}, Strategy: Switch, Result: {4}   ", door + 1, picked + 1, remaining + 1, result + 1, door == result ? "Won " : "Lost");
                    if (Visual)
                        Console.Write(Visualized(pDoors, door, result, picked, remaining));
                    if (Text || Visual)
                        Console.WriteLine();
                    return door == result;
            }

            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Unrecognized Strategy");
            return false;
        }

        static string Visualized(int doors, int winner, int result, int initial, int remaining)
        {
            StringBuilder sb = new StringBuilder();
            for(int i = 0; i < doors; i++)
            {
                if (i == winner)
                    sb.Append("(W)");
                else if (i == remaining || i == initial)
                    sb.Append("( )");
                else
                    sb.Append("(x)");

                if (i == result)
                    sb.Append("*");
                else if (i == initial)
                    sb.Append(".");
                else
                    sb.Append(" ");

                sb.Append(" ");
            }
            return sb.ToString();
        }
    }
}
