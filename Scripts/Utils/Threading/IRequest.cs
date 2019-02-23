namespace Utils.Threading
{
    public interface IRequest
    {
        void Process();
    }
    public interface IRequest<T1>
    {
        void Process(T1 val);
    }
    public interface IRequest<T1, T2>
    {
        void Process(T1 val, T2 val2);
    }
    public interface IRequest<T1, T2, T3>
    {
        void Process(T1 val, T2 val2, T3 val3);
    }
}
