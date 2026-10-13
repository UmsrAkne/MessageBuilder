using MessageBuilder.Services;
using MessageBuilder.ViewModels;

namespace MessageBuilder.Tests.Services
{
    [TestFixture]
    public class TextListSerializerTests
    {
        private readonly string testDir;
        private readonly TextListSerializer sut; // System Under Test

        public TextListSerializerTests()
        {
            // テストごとにユニークな一時フォルダを作成
            testDir = Path.Combine(Path.GetTempPath(), "TextListTest_" + Guid.NewGuid().ToString("N"));
            sut = new TextListSerializer(testDir);
        }

        [Test]
        public void SaveAndLoad_ValidViewModels_RestoresOriginalData()
        {
            // 1. Arrange: 保存用のテストデータを用意
            var originalVm = new TextListViewModel(null);
            originalVm.TabHeader = "テストタイトル（日本語）";
            originalVm.TextListItems.Add(new TextListItem("アイテム1"));
            originalVm.TextListItems.Add(new TextListItem("アイテム2"));

            // 2. Act: 実際にファイル保存を実行
            sut.Serialize(new[] { originalVm, });

            // 保存されたファイルを特定（1件保存されたはず）
            var savedFiles = Directory.GetFiles(testDir, "*.json");
            Assert.That(savedFiles.Length, Is.EqualTo(1));

            // 復元先の空のViewModelにロード
            var restoredVm = new TextListViewModel(null);
            sut.DeserializeInto(savedFiles[0], restoredVm);

            // 3. Assert: 元通り復元されているか一連の動作を検証
            Assert.That(originalVm.TabHeader, Is.EqualTo(restoredVm.TabHeader));
            Assert.That(originalVm.TextListItems.Count, Is.EqualTo(restoredVm.TextListItems.Count));
            Assert.That(originalVm.TextListItems[0].Text, Is.EqualTo(restoredVm.TextListItems[0].Text));
        }

        [TearDown]
        public void TearDown()
        {
            if (Directory.Exists(testDir))
            {
                Directory.Delete(testDir, recursive: true);
            }
        }
    }
}