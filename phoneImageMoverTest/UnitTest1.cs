using phoneImageMover;

namespace phoneImageMoverTest
{
    public class Tests
    {
        private Options options;
        private Processor processor;
        private Logger logger;
        private FileNameOperations fileNameOperations;
        
        [SetUp]
        public void Setup()
        {
            options = new Options();
            processor = new Processor();
            logger = new Logger(false);
            fileNameOperations = new FileNameOperations(logger);
        }

        [Test]
        public void TestIMGYearIndex()
        {
            int result = fileNameOperations.getIndexOfYearInFilename("IMG_20220201.png");
            Assert.That(result, Is.EqualTo(4));
        }
        
        [Test]
        public void TestPHOYearIndex()
        {
            int result = fileNameOperations.getIndexOfYearInFilename("PHO___20220201.png");
            Assert.That(result, Is.EqualTo(6));
        }

        [Test]
        public void TestVIDYearIndex()
        {
            int result = fileNameOperations.getIndexOfYearInFilename("VID1111_20220201.png");
            Assert.That(result, Is.EqualTo(8));
        }

        [Test]
        public void TestUnrecognizedYearIndex()
        {
            int result = fileNameOperations.getIndexOfYearInFilename("nope_20220201.png");
            Assert.That(result, Is.EqualTo(5));
        }

        [Test]
        public void TestFilename()
        {
            string result = fileNameOperations.buildDestinationPathByFilename("IMG_20220101.png", "C:\\root\\");
            Assert.That(result, Is.EqualTo("C:\\root\\2022\\01\\IMG_20220101.png"));
        }

        [Test]
        public void TestMp4Filename()
        {
            DateTime dateTime = new DateTime(2022, 2, 1);
            DateTime result = fileNameOperations.GetDateForMp4Filename("0123456_20220201.mp4", 8);
            Assert.That(result, Is.EqualTo(dateTime));
        }
    }
}