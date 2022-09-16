using MailContainerTest.Data;
using MailContainerTest.Services;
using MailContainerTest.Types;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MailContainerTest.Tests
{
    [TestClass]
    public class UnitTest
    {
        MakeMailTransferRequest makeMailTransferRequest;
        public UnitTest()
        {
            makeMailTransferRequest = new MakeMailTransferRequest();
            makeMailTransferRequest.NumberOfMailItems = 1;
            makeMailTransferRequest.SourceMailContainerNumber = "5";
            makeMailTransferRequest.DestinationMailContainerNumber = "6";
            makeMailTransferRequest.TransferDate = DateTime.Now;
            makeMailTransferRequest.MailType = MailType.SmallParcel;
        }

        /// <summary>
        /// To test the availability of Source Container
        /// </summary>
        [TestMethod]
        public void TestSourceContainerAvailability()
        {
            //Arrange
            MailContainerDataStore mailSourceContainerDataStore = new MailContainerDataStore();

            //Act
            var result = mailSourceContainerDataStore.GetMailContainer(makeMailTransferRequest.SourceMailContainerNumber);

            //Assert
            Assert.IsTrue(result.Status.Equals(MailContainerStatus.Operational));
        }

        /// <summary>
        /// To test the availability of Destination Container
        /// </summary>
        [TestMethod]
        public void TestDestinationContainerAvailability()
        {
            //Arrange
            MailContainerDataStore mailDestinationContainerDataStore = new MailContainerDataStore();

            //Act
            var result = mailDestinationContainerDataStore.GetMailContainer(makeMailTransferRequest.DestinationMailContainerNumber);

            //Assert
            Assert.IsTrue(result.Status.Equals(MailContainerStatus.Operational));
        }

        /// <summary>
        /// To test container capacity
        /// </summary>
        [TestMethod]
        public void TestContainerContainerCapacity()
        {
            //Arrange
            MailContainerDataStore mailContainerDataStore = new MailContainerDataStore();

            //Act            
            var sourceContainer = mailContainerDataStore.GetMailContainer(makeMailTransferRequest.SourceMailContainerNumber);
            var destinationContainer = mailContainerDataStore.GetMailContainer(makeMailTransferRequest.DestinationMailContainerNumber);

            //Reducing the capacity of source container
            sourceContainer.Capacity -= makeMailTransferRequest.NumberOfMailItems;
            sourceContainer.AllowedMailType = AllowedMailType.SmallParcel;
            mailContainerDataStore.UpdateMailContainer(sourceContainer);

            //Increase the capacity of destination container like source
            destinationContainer.Capacity = sourceContainer.Capacity;
            destinationContainer.AllowedMailType = AllowedMailType.SmallParcel;
            mailContainerDataStore.UpdateMailContainer(destinationContainer);
            
            //Assert
            Assert.AreEqual(sourceContainer.Capacity, destinationContainer.Capacity);
        }

        /// <summary>
        /// To test Mail Transfer
        /// </summary>
        [TestMethod]
        public void TestMailTransfer()
        {
            //Arrange
            MailTransferService mailTransferService = new MailTransferService();
            MailContainerDataStore mailContainerDataStore = new MailContainerDataStore();

            var sourceContainer = mailContainerDataStore.GetMailContainer(makeMailTransferRequest.SourceMailContainerNumber);
            var destinationContainer = mailContainerDataStore.GetMailContainer(makeMailTransferRequest.DestinationMailContainerNumber);
            sourceContainer.Capacity -= makeMailTransferRequest.NumberOfMailItems;
            sourceContainer.AllowedMailType = AllowedMailType.SmallParcel;
            destinationContainer.Capacity = sourceContainer.Capacity;
            destinationContainer.AllowedMailType = AllowedMailType.SmallParcel;
            mailContainerDataStore.UpdateMailContainer(sourceContainer);
            mailContainerDataStore.UpdateMailContainer(destinationContainer);

            //Act
            var result = mailTransferService.MakeMailTransfer(makeMailTransferRequest);

            //Assert
            Assert.IsFalse(result.Success);
        }
    }
}
