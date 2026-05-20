CREATE OR ALTER PROCEDURE dbo.sp_FindAvailableUnitsByDateRange
    @TouristSiteId INT,
    @CheckInDate DATE,
    @CheckOutDate DATE
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        au.Id AS AccommodationUnitId,
        au.Code AS UnitCode,
        au.Name AS UnitName,
        au.MaxCapacity,
        au.BedroomCount AS Bedrooms,
        atp.Name AS AccommodationType
    FROM AccommodationUnits au
    INNER JOIN AccommodationTypes atp ON atp.Id = au.AccommodationTypeId
    WHERE au.TouristSiteId = @TouristSiteId
      AND au.IsActive = 1
      AND NOT EXISTS (
        SELECT 1
        FROM ReservationUnits ru
        INNER JOIN Reservations r ON r.Id = ru.ReservationId
        WHERE ru.AccommodationUnitId = au.Id
          AND r.Status <> 'Cancelled'
          AND r.CheckInDate < @CheckOutDate
          AND r.CheckOutDate > @CheckInDate
      );
END
GO

CREATE OR ALTER PROCEDURE dbo.sp_FindAvailableUnitsByDateAndPeople
    @TouristSiteId INT,
    @CheckInDate DATE,
    @CheckOutDate DATE,
    @People INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        au.Id AS AccommodationUnitId,
        au.Code AS UnitCode,
        au.Name AS UnitName,
        au.MaxCapacity,
        au.BedroomCount AS Bedrooms,
        atp.Name AS AccommodationType
    FROM AccommodationUnits au
    INNER JOIN AccommodationTypes atp ON atp.Id = au.AccommodationTypeId
    WHERE au.TouristSiteId = @TouristSiteId
      AND au.IsActive = 1
      AND au.MaxCapacity >= @People
      AND NOT EXISTS (
        SELECT 1
        FROM ReservationUnits ru
        INNER JOIN Reservations r ON r.Id = ru.ReservationId
        WHERE ru.AccommodationUnitId = au.Id
          AND r.Status <> 'Cancelled'
          AND r.CheckInDate < @CheckOutDate
          AND r.CheckOutDate > @CheckInDate
      );
END
GO

CREATE OR ALTER PROCEDURE dbo.sp_GetRatesBySiteSeasonPeopleAccommodation
    @TouristSiteId INT,
    @ReferenceDate DATE,
    @People INT,
    @AccommodationTypeId INT,
    @AccommodationUnitId INT = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        rp.Id AS RatePlanId,
        ts.Name AS SiteName,
        s.Name AS SeasonName,
        atp.Name AS AccommodationType,
        au.Name AS UnitName,
        rp.MinPeople,
        rp.MaxPeople,
        rp.BasePrice,
        rp.AdditionalPersonPrice
    FROM RatePlans rp
    INNER JOIN TouristSites ts ON ts.Id = rp.TouristSiteId
    INNER JOIN Seasons s ON s.Id = rp.SeasonId
    INNER JOIN AccommodationTypes atp ON atp.Id = rp.AccommodationTypeId
    LEFT JOIN AccommodationUnits au ON au.Id = rp.AccommodationUnitId
    WHERE rp.IsActive = 1
      AND rp.TouristSiteId = @TouristSiteId
      AND rp.AccommodationTypeId = @AccommodationTypeId
      AND (@AccommodationUnitId IS NULL OR rp.AccommodationUnitId = @AccommodationUnitId OR rp.AccommodationUnitId IS NULL)
      AND @ReferenceDate BETWEEN s.StartDate AND s.EndDate
      AND @People BETWEEN rp.MinPeople AND rp.MaxPeople
    ORDER BY rp.AccommodationUnitId DESC, rp.MinPeople DESC;
END
GO

CREATE OR ALTER PROCEDURE dbo.sp_CalculateTotalRate
    @TouristSiteId INT,
    @ReferenceDate DATE,
    @People INT,
    @AccommodationTypeId INT,
    @RoomCount INT,
    @Nights INT
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @BasePrice DECIMAL(18,2);
    DECLARE @AdditionalPersonPrice DECIMAL(18,2);
    DECLARE @IncludedPeople INT;
    DECLARE @ExtraPeople INT;
    DECLARE @SubtotalPerNight DECIMAL(18,2);

    SELECT TOP 1
        @BasePrice = rp.BasePrice,
        @AdditionalPersonPrice = rp.AdditionalPersonPrice,
        @IncludedPeople = rp.MaxPeople
    FROM RatePlans rp
    INNER JOIN Seasons s ON s.Id = rp.SeasonId
    WHERE rp.IsActive = 1
      AND rp.TouristSiteId = @TouristSiteId
      AND rp.AccommodationTypeId = @AccommodationTypeId
      AND @ReferenceDate BETWEEN s.StartDate AND s.EndDate
      AND @People BETWEEN rp.MinPeople AND rp.MaxPeople
    ORDER BY rp.MinPeople DESC;

    IF @BasePrice IS NULL
    BEGIN
        RAISERROR('No rate plan found for the provided criteria.', 16, 1);
        RETURN;
    END

    SET @ExtraPeople = CASE WHEN @People > @IncludedPeople THEN (@People - @IncludedPeople) ELSE 0 END;
    SET @SubtotalPerNight = (@BasePrice + (@AdditionalPersonPrice * @ExtraPeople)) * @RoomCount;

    SELECT
        @SubtotalPerNight AS SubtotalPerNight,
        @SubtotalPerNight * @Nights AS TotalAmount,
        @Nights AS Nights,
        @RoomCount AS RoomCount;
END
GO
