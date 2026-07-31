var mockRepo = new Mock<INicknameRepository>();
mockRepo.Setup(r => r.IsTaken("용사")).Returns(true);

var service = new NicknameService(mockRepo.Object); 