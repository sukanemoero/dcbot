namespace dcbot.General.Components;

public interface IHasButtons
{
    public ButtonIds Buttons { get; set; }

    public class ButtonIds(
        string first = null,
        string second = null,
        string third = null,
        string forth = null,
        string fifth = null,
        string sixth = null)
    {
        public readonly string First = first;
        public readonly string Second = second;
        public readonly string Third = third;
        public readonly string Forth = forth;
        public readonly string Fifth = fifth;
        public readonly string Sixth = sixth;

        public int IdContent(string check)
        {
            return check == null
                ? -1
                : check.Equals(First)
                    ? 0
                    : check.Equals(Second)
                        ? 1
                        : check.Equals(Third)
                            ? 2
                            : check.Equals(Forth)
                                ? 3
                                : check.Equals(Fifth)
                                    ? 4
                                    : check.Equals(Sixth)
                                        ? 5
                                        : -1;
        }
    };
}