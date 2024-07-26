using System;
using System.Collections.Generic;
using System.Text;

namespace CronicleClient.Models;

public record SearchScheduleRequest(string? title, string? parameterKeyName);
