public interface ITimeable
{
    void Accelerate(); // 가속 (미래로)
    void Revert();     // 회귀 (과거로)
    void SetFocus(bool isFocused); // 포커스 온/오프 (시각적 효과 제어)
}