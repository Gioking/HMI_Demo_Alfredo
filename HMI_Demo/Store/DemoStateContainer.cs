namespace HMI_Demo.Store
{
    public class DemoStateContainer
    {
        public bool Value { get; set; }

        public event Action OnBooleanValueChanges;

        public void SetBooleanValue(bool value)
        {
            Value = value;
            OnBooleanValueChanges?.Invoke();
        }     

    }
}
