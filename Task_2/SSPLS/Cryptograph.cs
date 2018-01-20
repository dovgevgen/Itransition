using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;

namespace RSPLS
{
    class Cryptograph
    {
        private HMACSHA512 _hmac;
        private string _keyString;

        public string GetKey
        {
            get { return _keyString; }
        }

        public Cryptograph()
        {
            _hmac = new HMACSHA512();
        }

        public void CreateHashFunction()
        {
            byte[] key = new Byte[64];
            RandomNumberGenerator rng = RandomNumberGenerator.Create();
            rng.GetBytes(key);
            _keyString = String.Concat(Array.ConvertAll(key, x => x.ToString("x2")));
            _hmac.Key = Encoding.ASCII.GetBytes(_keyString);
        }

        public string ComputeHash(string massage)
        {
            byte[] hash = _hmac.ComputeHash(Encoding.ASCII.GetBytes(massage));
            return String.Concat(Array.ConvertAll(hash, x => x.ToString("x2")));
        }
    }
}
