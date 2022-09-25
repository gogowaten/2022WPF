using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
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


    //    【Unity】Unityで学ぶStrategyパターン！モンスターに色んな技を使わせよう！【C#】【プログラム設計】 | サプライドの技術者BLOG
    //https://techlife.supride.jp/archives/1411
    //より
    public class StrategyStructure
    {
        private Monster? _kawamon;

        public void Start()
        {
            //カワノバナというモンスター生成
            _kawamon = new Monster("カワノバナ")
            {
                //カワノバナに技追加
                Waza1 = new Hikkaku(),
                Waza2 = new KawanoCutter()
            };
            //カワノカッター攻撃
            _kawamon.Attack(_kawamon.Waza2);


            //カワノバナの技１を毒爆弾に変更
            _kawamon.Waza1 = new PoisonBom();
            //毒爆弾攻撃
            _kawamon.Attack(_kawamon.Waza1);


            //カワノカゲというモンスター生成
            _kawamon = new Monster("カワノカゲ")
            {
                Waza1 = new Hinoko(),
                Waza2 = new Hikkaku()
            };
            //ひのこ攻撃
            _kawamon.Attack(_kawamon.Waza1);
        }
    }

    /// <summary>
    /// 技の抽象クラス
    /// </summary>
    abstract class Waza
    {
        /// <summary>
        /// 技の効果
        /// </summary>
        public abstract void Skilleffect();
        /// <summary>
        /// 技の威力
        /// </summary>
        public int damage;
        /// <summary>
        /// 技の名前
        /// </summary>
        public string? waza_name;
        /// <summary>
        /// 技のタイプ　炎とか水とか雷とか
        /// </summary>
        public string? waza_type;
    }
    /// <summary>
    /// 技名：ひっかく　技タイプ：ノーマル　ダメージ：３０
    /// </summary>
    class Hikkaku : Waza
    {
        public Hikkaku()
        {
            this.damage = 30;
            this.waza_name = "ひっかく";
            this.waza_type = "ノーマル";
        }

        public override void Skilleffect()
        {
            Debug.WriteLine("技効果なし！");
        }

    }

    /// <summary>
    /// 技名：カワノカッター　技タイプ：草　ダメージ：60
    /// </summary>
    class KawanoCutter : Waza
    {
        // Constructor
        public KawanoCutter()
        {
            this.damage = 60;
            this.waza_name = "カワノカッター";
            this.waza_type = "草";
        }

        public override void Skilleffect()
        {
            Debug.WriteLine("クリティカル率２倍");
        }
    }

    /// <summary>
    /// 技名：毒爆弾　技タイプ：毒　ダメージ：90
    /// </summary>
    class PoisonBom : Waza
    {
        // Constructor
        public PoisonBom()
        {
            this.damage = 90;
            this.waza_name = "毒爆弾";
            this.waza_type = "毒";
        }

        public override void Skilleffect()
        {
            Debug.WriteLine("相手を毒状態にする");
        }
    }

    /// <summary>
    /// 技名：火の粉　技タイプ：炎　ダメージ：40
    /// </summary>
    class Hinoko : Waza
    {
        // Constructor
        public Hinoko()
        {
            this.damage = 40;
            this.waza_name = "火の粉";
            this.waza_type = "炎";
        }

        public override void Skilleffect()
        {
            Debug.WriteLine("相手を火傷状態にする");
        }
    }

    /// <summary>
    /// モンスタークラス　モンスターの名前と技を初期化
    /// </summary>
    class Monster
    {
        private string? name;
        public Monster(string name) { this.name = name; }
        public Waza? Waza1 { get; set; }
        public Waza? Waza2 { get; set; }

        //モンスターの攻撃
        public void Attack(Waza _waza)
        {
            Debug.WriteLine(this.name + "の攻撃！　" + _waza.damage + " ダメージ！");
            //攻撃後の効果
            _waza.Skilleffect();
        }

    }

}
