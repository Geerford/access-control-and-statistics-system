﻿using Domain.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Service.DTO;
using Service.Interfaces;
using sstu_nevdev.Controllers;
using sstu_nevdev.Models;
using System;
using System.Collections.Generic;
using System.Net;
using System.Web.Http.Results;
using System.Web.Mvc;
using RedirectToRouteResult = System.Web.Mvc.RedirectToRouteResult;

namespace sstu_nevdev.Tests.Controllers
{
    [TestClass]
    public class ActivityControllerTest
    {
        private Mock<IActivityService> activityServiceMock;
        private Mock<ICheckpointService> checkpointServiceMock;
        private Mock<IIdentityService> identityServiceMock;
        private ActivityController controllerWEB;
        private ActivitiesController controllerAPI;
        private List<Activity> items;
        private List<CheckpointDTO> checkpoints;
        private List<IdentityDTO> users;
        private readonly int id = 1;
        
        [TestInitialize]
        public void Initialize()
        {
            activityServiceMock = new Mock<IActivityService>();
            checkpointServiceMock = new Mock<ICheckpointService>();
            identityServiceMock = new Mock<IIdentityService>();
            controllerWEB = new ActivityController(activityServiceMock.Object);
            controllerAPI = new ActivitiesController(activityServiceMock.Object, checkpointServiceMock.Object,
                identityServiceMock.Object);
            items = new List<Activity>()
            {
                new Activity
                {
                    IdentityGUID = "1",
                    CheckpointIP = "192.168.0.1",
                    Date = DateTime.Now,
                    Mode = "Вход",
                    Status = true
                },
                new Activity
                {
                    IdentityGUID = "2",
                    CheckpointIP = "192.168.0.2",
                    Date = DateTime.Now,
                    Mode = "Выход",
                    Status = true
                },
                new Activity
                {
                    IdentityGUID = "3",
                    CheckpointIP = "192.168.0.3",
                    Date = DateTime.Now,
                    Mode = "Вход",
                    Status = true
                }
            };
            checkpoints = new List<CheckpointDTO>()
            {
                new CheckpointDTO
                {
                    IP = "192.168.0.1",
                    Campus = 1,
                    Row = 4,
                    Description = "На 4 этаже, 425 аудитория",
                    Status = "Отметка",
                    Type = new TypeDTO {
                        ID = 1,
                        Description = "Пропускает через ворота",
                        Status = "Пропускной"
                    },
                    Admissions = new List<Admission>(){
                        new Admission {
                            Role = "Сотрудник",
                            Description = "Вход в лабораторию"
                        }
                    }
                },
                new CheckpointDTO
                {
                    IP = "192.168.0.15",
                    Campus = 1,
                    Row = 4,
                    Description = "На 1 этаже на входе",
                    Status = "Пропуск",
                    Type = new TypeDTO {
                        ID = 2,
                        Description = "Отмечает посещаемость",
                        Status = "Лекционный"
                    },
                    Admissions = new List<Admission>(){
                        new Admission {
                            Role = "Сотрудник",
                            Description = "Вход в лабораторию"
                        },
                        new Admission {
                            Role = "Студент",
                            Description = "Вход в 1-й корпус"
                        }
                    }
                },
                new CheckpointDTO
                {
                    IP = "192.168.0.25",
                    Campus = 1,
                    Row = 4,
                    Description = "На 1 этаже на входе",
                    Status = "Отметка",
                    Type = new TypeDTO {
                        ID = 3,
                        Description = "Собирает статистику",
                        Status = "Статистический"
                    },
                    Admissions = new List<Admission>()
                }
            };
            users = new List<IdentityDTO>()
            {
                new IdentityDTO()
                {
                    ID = 1,
                    GUID = "1",
                    Picture = "cat.jpg",
                    Name = "Сидр",
                    Surname = "Сидоров",
                    Midname = "Сидорович",
                    Gender = true,
                    Birthdate = Convert.ToDateTime("2000-01-25"),
                    Country = "Россия",
                    City = "Саратов",
                    Department = "ИнПИТ",
                    Group = "ИФСТ",
                    Status = "Отчислен",
                    Email = "email@gmail.com",
                    Phone = "+79993499334",
                    Role = "Студент"
                },
                new IdentityDTO()
                {
                    ID = 2,
                    GUID = "2",
                    Picture = "cat.jpg",
                    Name = "Петр",
                    Surname = "Петров",
                    Midname = "Петрович",
                    Gender = true,
                    Birthdate = Convert.ToDateTime("2000-01-25"),
                    Country = "Россия",
                    City = "Саратов",
                    Department = "ИнПИТ",
                    Group = "ИФСТ",
                    Status = "Обучающийся",
                    Email = "email@gmail.com",
                    Phone = "+79993499334",
                    Role = "Студент"
                },
                new IdentityDTO()
                {
                    ID = 3,
                    GUID = "3",
                    Picture = "cat.jpg",
                    Name = "Иван",
                    Surname = "Иванов",
                    Midname = "Иванович",
                    Gender = true,
                    Birthdate = Convert.ToDateTime("2000-01-25"),
                    Country = "Россия",
                    City = "Саратов",
                    Department = "ИнПИТ",
                    Group = "ИФСТ",
                    Status = "Отпуск",
                    Email = "email@gmail.com",
                    Phone = "+79993499334",
                    Role = "Преподаватель"
                }
            };
        }

        [TestMethod]
        public void Activity_Index()
        {
            //Arrange
            activityServiceMock.Setup(x => x.GetAll()).Returns(items);

            //Act
            var result = ((controllerWEB.Index() as ViewResult).Model) as List<Activity>;

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(items, result);
        }

        [TestMethod]
        public void Activity_Get_Details()
        {
            //Arrange
            activityServiceMock.Setup(x => x.Get(id)).Returns(items[0]);

            //Act
            var result = ((controllerWEB.Details(id) as ViewResult).Model) as Activity;

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(items[0], result);
        }

        [TestMethod]
        public void Activity_Create_Valid()
        {
            //Arrange
            ActivityViewModel model = new ActivityViewModel() {
                IdentityGUID = "1",
                CheckpointIP = "192.168.0.0",
                Date = DateTime.Now,
                Status = "Успех",
                Mode = "Вход"
            };

            //Act
            var result = (RedirectToRouteResult)controllerWEB.Create(model);

            //Assert 
            Assert.AreEqual("Index", result.RouteValues["action"]);
        }

        [TestMethod]
        public void Activity_Create_Invalid()
        {
            // Arrange
            ActivityViewModel model = new ActivityViewModel()
            {
                IdentityGUID = "1",
                CheckpointIP = "192.168.0.0",
                Status = "Успех",
                Mode = "Вход"
            };
            controllerWEB.ModelState.AddModelError("Error", "Что-то пошло не так");

            //Act
            var result = (ViewResult)controllerWEB.Create(model);

            //Assert
            Assert.AreEqual("", result.ViewName);
        }

        [TestMethod]
        public void Activity_Edit_Valid()
        {
            //Arrange
            ActivityViewModel model = new ActivityViewModel()
            {
                IdentityGUID = "1",
                CheckpointIP = "192.168.0.0",
                Date = DateTime.Now,
                Status = "Успех",
                Mode = "Вход"
            };

            //Act
            var result = (RedirectToRouteResult)controllerWEB.Edit(model);

            //Assert 
            Assert.AreEqual("Index", result.RouteValues["action"]);
        }

        [TestMethod]
        public void Activity_Edit_Invalid()
        {
            //Arrange
            ActivityViewModel model = new ActivityViewModel() { };
            controllerWEB.ModelState.AddModelError("Error", "Что-то пошло не так");

            //Act
            var result = (ViewResult)controllerWEB.Edit(model);

            //Assert
            Assert.AreEqual("", result.ViewName);
        }

        [TestMethod]
        public void Activity_Delete()
        {
            //Arrange
            Activity model = new Activity()
            {
                IdentityGUID = "1",
                CheckpointIP = "192.168.0.0",
                Date = DateTime.Now,
                Status = true,
                Mode = "Вход"
            };

            //Act
            var result = (RedirectToRouteResult)controllerWEB.Delete(model);

            //Assert 
            Assert.IsNotNull(result);
            Assert.AreEqual("Index", result.RouteValues["action"]);
        }

        [TestMethod]
        public void Activity_Get_All_API()
        {
            //Arrange
            activityServiceMock.Setup(x => x.GetAll()).Returns(items);

            //Act
            var actionResult = controllerAPI.Get();
            var createdResult = actionResult as OkNegotiatedContentResult<IEnumerable<Activity>>;

            //Assert
            Assert.IsNotNull(createdResult);
            Assert.IsInstanceOfType(createdResult.Content, typeof(IEnumerable<Activity>));
            Assert.AreEqual(items, createdResult.Content);
        }

        [TestMethod]
        public void Activity_Get_API()
        {
            //Arrange
            activityServiceMock.Setup(x => x.Get(id)).Returns(items[0]);

            //Act
            var response = controllerAPI.Get(id);
            var result = response as OkNegotiatedContentResult<Activity>;

            //Assert
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Content);
            Assert.IsInstanceOfType(result.Content, typeof(Activity));
        }

        [TestMethod]
        public void Activity_Get_Not_Found_API()
        {
            //Act
            var result = controllerAPI.Get(100);

            //Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }

        [TestMethod]
        public void Activity_Post_Statistical_Valid_API()
        {
            //Arrange
            activityServiceMock.Setup(x => x.GetAll()).Returns(items);
            activityServiceMock.Setup(x => x.IsOk(checkpoints[2], users[0])).Returns(-1);
            checkpointServiceMock.Setup(x => x.GetByIP("192.168.0.25")).Returns(checkpoints[2]);
            identityServiceMock.Setup(x => x.GetByGUID(id.ToString())).Returns(users[0]);
            ActivityAPIModel item = new ActivityAPIModel
            {
                GUID = "1",
                CheckpointIP = "192.168.0.25"
            };

            //Act
            var actionResult = controllerAPI.Post(item);
            var createdResult = actionResult as OkResult;

            //Assert
            Assert.IsNotNull(createdResult);
        }
        
        [TestMethod]
        public void Activity_Post_Classroom_Valid_API()
        {
            //Arrange
            activityServiceMock.Setup(x => x.GetAll()).Returns(items);
            activityServiceMock.Setup(x => x.IsOk(checkpoints[1], users[0])).Returns(-1);
            checkpointServiceMock.Setup(x => x.GetByIP("192.168.0.15")).Returns(checkpoints[1]);
            identityServiceMock.Setup(x => x.GetByGUID(id.ToString())).Returns(users[0]);
            ActivityAPIModel item = new ActivityAPIModel
            {
                GUID = "1",
                CheckpointIP = "192.168.0.15"
            };

            //Act
            var actionResult = controllerAPI.Post(item);
            var createdResult = actionResult as OkResult;

            //Assert
            Assert.IsNotNull(actionResult);
        }

        [TestMethod]
        public void Activity_Post_Security_Allowed_Valid_API()
        {
            //Arrange
            activityServiceMock.Setup(x => x.GetAll()).Returns(items);
            activityServiceMock.Setup(x => x.IsOk(checkpoints[0], users[0])).Returns(200);
            checkpointServiceMock.Setup(x => x.GetByIP("192.168.0.1")).Returns(checkpoints[0]);
            identityServiceMock.Setup(x => x.GetByGUID(id.ToString())).Returns(users[0]);
            ActivityAPIModel item = new ActivityAPIModel
            {
                GUID = "1",
                CheckpointIP = "192.168.0.1"
            };

            //Act
            var actionResult = controllerAPI.Post(item);
            var createdResult = actionResult as JsonResult<IdentityDTO>;

            //Assert
            Assert.IsNotNull(createdResult);
            Assert.IsNotNull(createdResult.Content);
            Assert.IsInstanceOfType(createdResult.Content, typeof(IdentityDTO));
        }

        [TestMethod]
        public void Activity_Post_Security_Prohibited_Valid_API()
        {
            //Arrange
            activityServiceMock.Setup(x => x.GetAll()).Returns(items);
            activityServiceMock.Setup(x => x.IsOk(checkpoints[0], users[0])).Returns(500);
            checkpointServiceMock.Setup(x => x.GetByIP("192.168.0.1")).Returns(checkpoints[0]);
            identityServiceMock.Setup(x => x.GetByGUID(id.ToString())).Returns(users[0]);
            ActivityAPIModel item = new ActivityAPIModel
            {
                GUID = "1",
                CheckpointIP = "192.168.0.1"
            };

            //Act
            var actionResult = controllerAPI.Post(item);
            var createdResult = actionResult as JsonResult<string>;

            //Assert
            Assert.IsNotNull(createdResult);
            Assert.IsNotNull(createdResult.Content);
            Assert.IsInstanceOfType(createdResult.Content, typeof(string));
        }

        [TestMethod]
        public void Activity_Post_Invalid_API()
        {
            //Arrange
            ActivityAPIModel item = null;

            //Act
            var actionResult = controllerAPI.Post(item);
            var createdResult = actionResult as BadRequestResult;

            //Assert
            Assert.IsNotNull(createdResult);
        }

        [TestMethod]
        public void Activity_Put_Valid_API()
        {
            //Arrange
            Activity item = new Activity()
            {
                ID = id,
                IdentityGUID = "1",
                CheckpointIP = "192.168.0.1",
                Date = DateTime.Now,
                Mode = "Вход",
                Status = true
            };

            //Act
            var actionResult = controllerAPI.Put(id, item);
            var contentResult = actionResult as NegotiatedContentResult<Activity>;

            //Assert
            Assert.IsNotNull(contentResult);
            Assert.IsNotNull(contentResult.Content);
            Assert.AreEqual(HttpStatusCode.OK, contentResult.StatusCode);
        }

        [TestMethod]
        public void Activity_Put_Invalid_API()
        {
            //Arrange
            int newid = 2;
            Activity item = new Activity()
            {
                ID = newid,
                IdentityGUID = "1",
                CheckpointIP = "192.168.0.1",
                Date = DateTime.Now,
                Mode = "Вход",
                Status = true
            };

            //Act
            var actionResult = controllerAPI.Put(id, item);
            var contentResult = actionResult as BadRequestResult;

            //Assert
            Assert.IsNotNull(contentResult);
        }

        [TestMethod]
        public void Activity_Delete_API()
        {
            //Arrange
            Activity item = new Activity()
            {
                ID = id,
                IdentityGUID = "1",
                CheckpointIP = "192.168.0.1",
                Date = DateTime.Now,
                Mode = "Вход",
                Status = true
            };
            activityServiceMock.Setup(x => x.Get(id)).Returns(item);

            //Act
            var actionResult = controllerAPI.Delete(id);
            var contentResult = actionResult as NegotiatedContentResult<Activity>;

            //Assert
            Assert.IsNotNull(contentResult);
            Assert.IsNotNull(contentResult.Content);
            Assert.AreEqual(HttpStatusCode.OK, contentResult.StatusCode);
            Assert.AreEqual(item, contentResult.Content);
        }
    }
}