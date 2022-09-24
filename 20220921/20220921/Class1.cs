using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


//デザインパターンの解説と実装例！ -えんじにあ雑記！
//https://www.mum-meblog.com/entry/study/design-pattern
//より

namespace _20220921
{
    internal class Class1
    {
    }
    //Strategyパターン
    public interface IAttack { void Attack(); }
    public class Hit : IAttack { public void Attack() { Debug.WriteLine("打撃"); } }
    public class Magic : IAttack { public void Attack() { Debug.WriteLine("魔法"); } }
    public class PlayerAttack
    {
        private IAttack attack;
        public PlayerAttack(IAttack attack) { this.attack = attack; }
        public void Attack() { attack.Attack(); }
        public void SetAttack(IAttack attack) { this.attack = attack; }
    }


    public interface ICommand
    {
        void Execute();
        void Undo();
    }
    public class Light
    {
        public void On() { Debug.WriteLine("オン"); }
        public void Off() { Debug.WriteLine("オフ"); }
    }
    public class LightOnCommand : ICommand
    {
        private Light light;
        public LightOnCommand(Light light) { this.light = light; }
        public void Execute() { light.On(); }
        public void Undo() { light.Off(); }
    }






}
