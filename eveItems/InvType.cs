namespace eveItems
{
    public class Name
    {
        public string en { get; set; }
    }
    public class InvType
    {
        public Name name { get; set; }

        public override string ToString() 
        {
            return name.en;
        }
    }
}
