using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;


//Factory Method を C# で / デザインパターン
//https://refactoring.guru/ja/design-patterns/factory-method/csharp/example#example-0
//より

namespace _20221016_デザインパターンFactoryMethod
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            new Client().Main();
        }
    }


    // Creator クラスは、Product クラスのオブジェクトを返すことになっているファクトリメソッドを宣言します。
    // Creator のサブクラスは通常、このメソッドの実装を提供します。
    abstract class Creator
    {
        // Creator は、ファクトリ メソッドのデフォルトの実装も提供する場合があることに注意してください。
        public abstract IProduct FactoryMethod();

        public string SomeOperation()
        {
            // Call the factory method to create a Product object.
            var product = FactoryMethod();
            // Now, use the product.
            var result = "Creator: 同じ作成者のコードがちょうど動作しました "
                + product.Operation();

            return result;
        }
    }

    // 具体的なクリエーターは、結果の製品のタイプを変更するために、ファクトリ メソッドをオーバーライドします。
    class ConcreteCreator1 : Creator
    {
        public override IProduct FactoryMethod()
        {
            return new ConcreteProduct1();
        }
    }

    class ConcreteCreator2 : Creator
    {
        public override IProduct FactoryMethod()
        {
            return new ConcreteProduct2();
        }
    }

    // Product インターフェイスは、すべての具体的な製品が実装する必要がある操作を宣言します。
    public interface IProduct
    {
        string Operation();
    }

    // Concrete Products provide various implementations of the Product interface.
    // 具体的な製品は、製品インターフェースのさまざまな実装を提供します。
    class ConcreteProduct1 : IProduct
    {
        public string Operation()
        {
            return "{Result of プロダクト1}";
        }
    }

    class ConcreteProduct2 : IProduct
    {
        public string Operation()
        {
            return "{Result of プロダクト2}";
        }
    }

    class Client
    {
        public void Main()
        {
            Debug.
            Debug.WriteLine("App: Launched with the クリエイター1.");
            ClientCode(new ConcreteCreator1());

            Debug.WriteLine("");

            Debug.WriteLine("App: Launched with the クリエイター2.");
            ClientCode(new ConcreteCreator2());
        }

        // The client code works with an instance of a concrete creator, albeit
        // through its base interface. As long as the client keeps working with
        // the creator via the base interface, you can pass it any creator's
        // subclass.
        // クライアント コードは、ベース インターフェイスを介してではあるが、
        // 具象クリエータのインスタンスで動作します。
        // クライアントがベース インターフェイスを介してクリエーターと連携し続ける限り、
        // 任意のクリエーターのサブクラスをクライアントに渡すことができます。
        public void ClientCode(Creator creator)
        {
            // ...
            Debug.WriteLine("Client: I'm not aware of the creator's class," +
                "but it still works.\n" + creator.SomeOperation());
            // ...
        }
    }

    //class Program
    //{
    //    static void Main(string[] args)
    //    {
    //        new Client().Main();
    //    }
    //}
}
