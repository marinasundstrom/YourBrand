﻿using Skynet.TimeReport.Application.Users;

namespace Skynet.TimeReport.Application.TimeSheets;

public record class TimeSheetDto(string Id, int Year, int Week, DateTime From, DateTime To, TimeSheetStatusDto Status, UserDto User, IEnumerable<TimeSheetActivityDto> Activities, IEnumerable<MonthInfoDto> Months);