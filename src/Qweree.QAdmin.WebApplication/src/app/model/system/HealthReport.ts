export class HealthReport {
  constructor(
    public status: string,
    public entries: HealthReportEntry[]
  ) {
    if (!entries) {
      this.entries = [];
    }
  }
}

export class HealthReportEntry {
  constructor(
    public status: string
  ) {
  }
}
