using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace luhn_check
{
    public class TagService
    {
        TagRepository _tagRepository;
        Random _random;

        public TagService()
        {
            _tagRepository = new TagRepository();
            _random = new Random();
        }

        public Tag GetUnusedPrimary()
        {
            int primary;
            var exists = false;
            do
            {
                primary = GeneratePrimary();
                exists = _tagRepository.ContainsPrimary(primary.ToString());
            }
            while (exists);

            var tag = CreateTag(primary);
            _tagRepository.Store(tag);

            return tag;
        }

        private int GeneratePrimary()
        {
            int rInt = _random.Next(1, 4194967);    // 41 not 42 to ensure we stay in range
            return rInt;
        }

        public Tag CreateTag(int seed)
        {
            var tag = new Tag();
            tag.Primary = seed.ToString();
            
            // positions of the string to check
            int[] check1Positions = new[] { 0, 1, 2, 3, 4 };
            int[] check2Positions = new[] { 2, 3, 4, 5, 6 };
            int[] check3Positions = new[] { 0, 1, 5, 6 };

            var checkString1 = GetCheckSequence(tag.PrimaryFullyQualifed, check1Positions);
            var checkString2 = GetCheckSequence(tag.PrimaryFullyQualifed, check2Positions);
            var checkString3 = GetCheckSequence(tag.PrimaryFullyQualifed, check3Positions);

            tag.CheckDigit1 = checkString1.CheckDigit();
            tag.CheckDigit2 = checkString2.CheckDigit();
            tag.CheckDigit3 = checkString3.CheckDigit();

            return tag;
        }



        private string GetCheckSequence(string primary, int[] positions)
        {
            var sb = new StringBuilder();
            foreach(var position in positions)
            {
                sb.Append(primary[position]);
            }
            return sb.ToString();
        }
    }

    public class Tag
    {
        public string Primary { get; set; }

        public string CheckDigit1 { get; set; }

        public string CheckDigit2 { get; set; }

        public string CheckDigit3 { get; set; }

        public string Full => $"{Primary}{CheckDigit1}{CheckDigit2}{CheckDigit3}";

        public string PrimaryFullyQualifed => GetFullyQualifed(Primary);

        public List<int> GetPrimaryDigits()
        {
            return PrimaryFullyQualifed.Select(d => d - 48).ToList(); // from luhn extensions
        }

        public override bool Equals(object obj)
        {
            var otherAsTag = obj as Tag;
            return otherAsTag != null && otherAsTag.PrimaryFullyQualifed.Equals(PrimaryFullyQualifed);
        }

        public static string GetFullyQualifed(string primary) => primary.PadLeft(7, '0');

        public override string ToString()
        {
            return $"{Primary} {CheckDigit1}{CheckDigit2}{CheckDigit3}";
        }
    }

    public class TagRepository
    {
        List<Tag> _values;

        public TagRepository()
        {
            _values = new List<Tag>();
        }

        public void Store(Tag value)
        {
            _values.Add(value);
        }

        public bool ContainsPrimary(string primary)
        {
            var fqPrimary= Tag.GetFullyQualifed(primary);
            return _values.Exists(v => v.PrimaryFullyQualifed == fqPrimary);
        }

        public bool ContainsPrimary(Tag value)
        {
            return _values.Exists(v => v.Equals(value));
        }
    }
}
