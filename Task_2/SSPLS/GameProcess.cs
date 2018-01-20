using System;
using System.Collections.Generic;
using System.Text;

namespace RSPLS
{
    class GameProcess
    {
        private int _machineChoice;
        private int _userChoice;
        private Cryptograph _cryptograph;
        string[] _item = { "Scissors", "Paper", "Rock", "Lizard", "Spock" };

        public GameProcess()
        {
            _machineChoice = _userChoice = 0;
            _cryptograph = new Cryptograph();
        }

        public void Start()
        {
            while (true)
            {
                _cryptograph.CreateHashFunction();
                MachineMove();
                Console.WriteLine("Make a move:\n0.Exit");
                for (int i = 1; i <= _item.Length; i++)
                {
                    Console.WriteLine(i + "." + _item[i - 1]);
                }
                UserMove();
                switch (_userChoice)
                {
                    case 0:
                        return;
                    default:
                        Console.WriteLine("Machine move: {0}", _item[_machineChoice]);
                        Console.WriteLine("You move: {0}", _item[_userChoice - 1]);
                        break;
                }
                ComputeMove();
                Console.WriteLine("Hash key: " + _cryptograph.GetKey);
                Console.ReadLine();
                Console.Clear();
            }
        }

        public void MachineMove()
        {
            Random rand = new Random(DateTime.Now.Millisecond);
            _machineChoice = rand.Next(0, _item.Length);
            Console.WriteLine("Machine move: " + _cryptograph.ComputeHash(_item[_machineChoice]));
        }

        public void UserMove()
        {
            while (!int.TryParse(Console.ReadLine(), out _userChoice) || _userChoice < 0 || _userChoice > _item.Length)
            {
                Console.WriteLine("Please, repeat");
            }
        }

        public void ComputeMove()
        {
            _userChoice -= 1;
            if (_machineChoice - _userChoice > 0)
            {
                if ((_machineChoice - _userChoice) % 2 == 0)
                {
                    Console.WriteLine("You lose");
                }
                else
                    Console.WriteLine("You won");
            }
            if (_machineChoice - _userChoice < 0)
            {

                if ((_machineChoice - _userChoice) % 2 == 0)

                    Console.WriteLine("You won");

                else
                    Console.WriteLine("You lose");
            }
            if (_userChoice == _machineChoice)
                Console.WriteLine("Draw");
        }
    }
}
