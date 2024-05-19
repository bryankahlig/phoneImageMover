using phoneImageMover;

namespace phoneImageMoverTest
{
    public class Tests
    {
        private Options options;
        private Processor processor;
        private Logger logger;
        private FileNameOperations fileNameOperations;
        private IFilenameValidator filenameValidator;
        
        [SetUp]
        public void Setup()
        {
            options = new Options();
            processor = new Processor();
            logger = new Logger(false);
            fileNameOperations = new FileNameOperations(logger);
            filenameValidator = new FilenameValidator();
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

        [Test]
        public void TestYearValidity()
        {
            void TestYear(string year, bool expected)
            {
                bool result = filenameValidator.IsValidFilenameYear(year);
                Assert.That(result, Is.EqualTo(expected));
            }

            TestYear("2022", true);
            TestYear("2023", true);
            TestYear("1923", true);
            TestYear("1800", false);
            TestYear("fred", false);
            TestYear("202", false);
            TestYear("20222", false);
            TestYear(".2134jsl", false);
        }

        // Test month validity
        [Test]
        public void TestMonthValidity()
        {
            void TestMonth(string month, bool expected)
            {
                bool result = filenameValidator.IsValidFilenameMonth(month);
                Assert.That(result, Is.EqualTo(expected));
            }

            TestMonth("01", true);
            TestMonth("12", true);
            TestMonth("00", false);
            TestMonth("13", false);
            TestMonth("1", false);
            TestMonth("1a", false);
            TestMonth("1.0", false);
            TestMonth("j.0", false);
        }
    }
}