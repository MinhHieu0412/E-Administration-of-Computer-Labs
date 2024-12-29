
CREATE TABLE Users (
    ID INT IDENTITY(1,1) PRIMARY KEY,            -- Khóa chính tự tăng
    UserName NVARCHAR(255),                      -- Tên người dùng, có thể NULL
    Email NVARCHAR(255),                         -- Email, có thể NULL
    Password NVARCHAR(255),                      -- Mật khẩu, có thể NULL
    Image NVARCHAR(255),                         -- Hình ảnh đại diện, có thể NULL
    Role NVARCHAR(255),                          -- Vai trò, có thể NULL
    Status BIT,                                  -- Trạng thái (1: Hoạt động, 0: Không hoạt động)
    CreatedAt DATETIME DEFAULT GETDATE(),        -- Thời gian tạo, mặc định là thời gian hiện tại
    UpdatedAt DATETIME DEFAULT GETDATE(),        -- Thời gian cập nhật, mặc định là thời gian hiện tại
    DepartmentID INT NOT NULL,                   -- Khóa ngoại liên kết với bảng Department
    CONSTRAINT FK_Users_Departments FOREIGN KEY (DepartmentID) REFERENCES Departments(ID) ON DELETE CASCADE
);


INSERT INTO [dbo].[Users] 
([UserName], [Email], [Password], [Image], [Role], [Status], [CreatedAt], [UpdatedAt], [DepartmentID])
VALUES
('jane_smith', 'jane.smith@email.com', '$2a$12$DH4HN/dDMjsKDvt3PK29K.YJgUMpKF7CV2XHvDoVFokETYz68buja', 'jane.jpg', 'Lecturer', 1, '2024-12-08 08:00:00', '2024-12-08 08:00:00', 2),
('robert_brown', 'robert.brown@email.com', '$2a$12$DH4HN/dDMjsKDvt3PK29K.YJgUMpKF7CV2XHvDoVFokETYz68buja', 'robert.jpg', 'Admin', 1, '2024-12-08 08:00:00', '2024-12-08 08:00:00', 3),
('linda_white', 'linda.white@email.com', '$2a$12$DH4HN/dDMjsKDvt3PK29K.YJgUMpKF7CV2XHvDoVFokETYz68buja', 'linda.jpg', 'Technician', 1, '2024-12-08 08:00:00', '2024-12-08 08:00:00', 4),
('michael_green', 'michael.green@email.com', '$2a$12$DH4HN/dDMjsKDvt3PK29K.YJgUMpKF7CV2XHvDoVFokETYz68buja', 'michael.jpg', 'Lecturer', 1, '2024-12-08 08:00:00', '2024-12-08 08:00:00', 5),
('emily_black', 'emily.black@email.com', '$2a$12$DH4HN/dDMjsKDvt3PK29K.YJgUMpKF7CV2XHvDoVFokETYz68buja', 'emily.jpg', 'Technician', 1, '2024-12-08 08:00:00', '2024-12-08 08:00:00', 1);
SELECT * FROM Users;

SELECT * FROM Users WHERE ID IN (1, 2, 3);

DELETE FROM Users;

INSERT INTO Departments (Name, CreatedAt, UpdatedAt)
VALUES ('IT Department', GETDATE(), GETDATE()),
       ('HR Department', GETDATE(), GETDATE()),
       ('Finance Department', GETDATE(), GETDATE());
Select * from Departments;

INSERT INTO Assignments (UserID, LabID, Date, TimeStart, TimeEnd, Notes, CreatedAt, UpdatedAt)
VALUES 
(1, 1, '2024-12-01', '09:00:00', '11:00:00', 'Assignment for maintenance', GETDATE(), GETDATE()),
(2, 2, '2024-12-02', '13:00:00', '15:00:00', 'Daily inspection', GETDATE(), GETDATE()),
(3, 1, '2024-12-03', '10:00:00', '12:00:00', 'Setup lab equipment', GETDATE(), GETDATE());
select * from Assignments;

INSERT INTO IssueReports (LabID, ReporterID, DepartmentID, Description, EquipmentID, Status, CreatedAt, UpdatedAt)
VALUES 
(1, 1, 1, 'Microscope lens broken', 1, 'Pending', GETDATE(), GETDATE()),
(2, 2, 2, '3D printer jammed', 2, 'In Progress', GETDATE(), GETDATE()),
(3, 3, 3, 'Projector bulb burned out', 3, 'Resolved', GETDATE(), GETDATE());

select * from IssueReports;
CREATE TABLE LabRequests (
    ID INT IDENTITY(1,1) PRIMARY KEY,              -- Khóa chính tự tăng
    DepartmentID INT NOT NULL,                     -- Khóa ngoại liên kết với bảng Departments
    RequestedByID INT NOT NULL,                    -- Khóa ngoại liên kết với bảng Users
    Purpose NVARCHAR(MAX) NOT NULL,               -- Mục đích yêu cầu, bắt buộc
    AdminResponse NVARCHAR(MAX) NULL,             -- Phản hồi của Admin, có thể NULL
    Status NVARCHAR(MAX) NOT NULL,                -- Trạng thái yêu cầu, bắt buộc
    CreatedAt DATETIME2(7) DEFAULT GETDATE(),     -- Thời gian tạo, mặc định thời gian hiện tại
    UpdatedAt DATETIME2(7) DEFAULT GETDATE(),     -- Thời gian cập nhật, mặc định thời gian hiện tại
    CONSTRAINT FK_LabRequests_Departments FOREIGN KEY (DepartmentID) REFERENCES Departments(ID) ON DELETE CASCADE,
    CONSTRAINT FK_LabRequests_Users FOREIGN KEY (RequestedByID) REFERENCES Users(ID)
);

-- Tạo chỉ mục không phân cụm trên cột DepartmentID
CREATE NONCLUSTERED INDEX IX_LabRequests_DepartmentID
ON LabRequests (DepartmentID ASC);

-- Tạo chỉ mục không phân cụm trên cột RequestedByID
CREATE NONCLUSTERED INDEX IX_LabRequests_RequestedByID
ON LabRequests (RequestedByID ASC);

INSERT INTO LabRequests (DepartmentID, RequestedByID, Purpose, AdminResponse, CreatedAt, UpdatedAt)
VALUES 
(1, 1, 'Request for new laptops', 'Approved', GETDATE(), GETDATE()),
(2, 2, 'Request for additional chairs', 'Pending', GETDATE(), GETDATE()),
(3, 3, 'Request for new software licenses', 'Denied', GETDATE(), GETDATE());

select * from LabRequests;
INSERT INTO Equipments (LabID, Name, SerialNumber, EquipmentDetails, Status, CreatedAt, UpdatedAt)
VALUES
-- 5 thiết bị cho LabID = 1
(1, 'Microscope-1',       'SN101', 'High precision optical microscope',   'Operational',         GETDATE(), GETDATE()),
(1, 'Centrifuge-1',       'SN102', 'High-speed centrifuge',               'Operational',         GETDATE(), GETDATE()),
(1, 'Spectrophotometer-1','SN103', 'UV-Vis spectrophotometer',           'Maintenance Required', GETDATE(), GETDATE()),
(1, 'Pipette Set-1',      'SN104', 'Micropipette set for measurements',   'Operational',         GETDATE(), GETDATE()),
(1, 'Incubator-1',        'SN105', 'Temperature controlled incubator',    'Operational',         GETDATE(), GETDATE()),

-- 5 thiết bị cho LabID = 2
(2, 'Microscope-2',       'SN201', 'High precision optical microscope',   'Operational',         GETDATE(), GETDATE()),
(2, 'Centrifuge-2',       'SN202', 'High-speed centrifuge',               'Operational',         GETDATE(), GETDATE()),
(2, 'Spectrophotometer-2','SN203', 'UV-Vis spectrophotometer',           'Maintenance Required', GETDATE(), GETDATE()),
(2, 'Pipette Set-2',      'SN204', 'Micropipette set for measurements',   'Operational',         GETDATE(), GETDATE()),
(2, 'Incubator-2',        'SN205', 'Temperature controlled incubator',    'Operational',         GETDATE(), GETDATE()),

-- 5 thiết bị cho LabID = 3
(3, 'Microscope-3',       'SN301', 'High precision optical microscope',   'Operational',         GETDATE(), GETDATE()),
(3, 'Centrifuge-3',       'SN302', 'High-speed centrifuge',               'Operational',         GETDATE(), GETDATE()),
(3, 'Spectrophotometer-3','SN303', 'UV-Vis spectrophotometer',           'Maintenance Required', GETDATE(), GETDATE()),
(3, 'Pipette Set-3',      'SN304', 'Micropipette set for measurements',   'Operational',         GETDATE(), GETDATE()),
(3, 'Incubator-3',        'SN305', 'Temperature controlled incubator',    'Operational',         GETDATE(), GETDATE()),

-- 5 thiết bị cho LabID = 4
(4, 'Microscope-4',       'SN401', 'High precision optical microscope',   'Operational',         GETDATE(), GETDATE()),
(4, 'Centrifuge-4',       'SN402', 'High-speed centrifuge',               'Operational',         GETDATE(), GETDATE()),
(4, 'Spectrophotometer-4','SN403', 'UV-Vis spectrophotometer',           'Maintenance Required', GETDATE(), GETDATE()),
(4, 'Pipette Set-4',      'SN404', 'Micropipette set for measurements',   'Operational',         GETDATE(), GETDATE()),
(4, 'Incubator-4',        'SN405', 'Temperature controlled incubator',    'Operational',         GETDATE(), GETDATE()),

-- 5 thiết bị cho LabID = 5
(5, 'Microscope-5',       'SN501', 'High precision optical microscope',   'Operational',         GETDATE(), GETDATE()),
(5, 'Centrifuge-5',       'SN502', 'High-speed centrifuge',               'Operational',         GETDATE(), GETDATE()),
(5, 'Spectrophotometer-5','SN503', 'UV-Vis spectrophotometer',           'Maintenance Required', GETDATE(), GETDATE()),
(5, 'Pipette Set-5',      'SN504', 'Micropipette set for measurements',   'Operational',         GETDATE(), GETDATE()),
(5, 'Incubator-5',        'SN505', 'Temperature controlled incubator',    'Operational',         GETDATE(), GETDATE());
    
SELECT * FROM Equipments;



CREATE TABLE Labs (
    ID INT IDENTITY(1,1) PRIMARY KEY,            -- Khóa chính tự tăng
    Name NVARCHAR(255) NOT NULL,                 -- Tên phòng, bắt buộc
    DepartmentID INT NOT NULL,                   -- Khóa ngoại liên kết với bảng Departments
    Description NVARCHAR(500) NOT NULL,          -- Mô tả phòng, bắt buộc
    Location NVARCHAR(100) NOT NULL,             -- Vị trí phòng, bắt buộc
    Capacity INT NOT NULL,                       -- Sức chứa, bắt buộc
    IsOperational BIT NOT NULL DEFAULT 1,        -- Trạng thái hoạt động, mặc định là TRUE
    CreatedAt DATETIME DEFAULT GETDATE(),        -- Thời gian tạo, mặc định là thời gian hiện tại
    UpdatedAt DATETIME DEFAULT GETDATE(),        -- Thời gian cập nhật, mặc định là thời gian hiện tại
    CONSTRAINT FK_Labs_Departments FOREIGN KEY (DepartmentID) REFERENCES Departments(ID) ON DELETE CASCADE
);
INSERT INTO Labs (Name, DepartmentID, Description, Location, Capacity, IsOperational, CreatedAt, UpdatedAt)
VALUES
('Physics Lab', 1, 'Lab for physics experiments', 'Building A, Room 101', 30, 1, GETDATE(), GETDATE()),
('Chemistry Lab', 2, 'Lab for chemistry research', 'Building B, Room 202', 25, 1, GETDATE(), GETDATE()),
('Biology Lab', 3, 'Lab for biology research', 'Building C, Room 303', 20, 1, GETDATE(), GETDATE()),
('Computer Lab', 4, 'Lab for computer programming', 'Building D, Room 404', 35, 1, GETDATE(), GETDATE()),
('Robotics Lab', 5, 'Lab for robotics projects', 'Building E, Room 505', 15, 0, GETDATE(), GETDATE());

SELECT * FROM Labs;



