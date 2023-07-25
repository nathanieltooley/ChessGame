using ChessGame.Scripts.ChessBoard;

namespace ChessTests
{
    [TestFixture]
    public class BoardTests
    {

        private LogicalBoard _board;

        [SetUp]
        public void Setup()
        {
            _board = new LogicalBoard();
        }

        [Test]
        public void Test1()
        {
            Assert.Pass();
        }
    }
}