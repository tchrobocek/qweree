import { Component, OnInit } from '@angular/core';
import {EnvironmentService} from '../../../services/environment/environment.service';
import {SystemAdapterService} from '../../../services/system/system-adapter.service';
import {HttpClient} from '@angular/common/http';
import {HealthReport} from '../../../model/system/HealthReport';

@Component({
  selector: 'app-services-overview',
  templateUrl: './services-overview.component.html',
  styleUrls: ['./services-overview.component.scss']
})
export class ServicesOverviewComponent implements OnInit {

  private readonly services: ServiceDescriptor[];
  public serviceReport: ServiceReport[];

  constructor(
    private environmentService: EnvironmentService,
    private httpClient: HttpClient
  ) {
    this.services = [{
      baseUri: 'http://localhost:10003/',
      name: `Qwill`
    }, {
      baseUri: environmentService.getEnvironment().authentication.baseUri,
      name: `OAuth`
    }, {
      baseUri: environmentService.getEnvironment().cdn.baseUri,
      name: `Cdn`
    }, {
      baseUri: '',
      name: `service2`
    }];
    this.serviceReport = [];
  }

  ngOnInit(): void {
    this.services.forEach(s => {
      const adapter = new SystemAdapterService(this.httpClient, s.baseUri);

      adapter.healthReport()
        .subscribe(h => {
          const report = this.getOrCreateServiceReport(s);
          report.healthReport = h;
        }, error => {
          const report = this.getOrCreateServiceReport(s);
          report.healthReport = new HealthReport(error.status, null);
        });
      adapter.versionReport()
        .subscribe(v => {
          const report = this.getOrCreateServiceReport(s);
          report.version = v.version;
        }, () => {
          const report = this.getOrCreateServiceReport(s);
          report.version = 'unknown';
        });
    });
  }

  getOrCreateServiceReport(service: ServiceDescriptor): ServiceReport {

    let serviceReport = this.serviceReport.find(r => r.name === service.name);
    if (!serviceReport) {
      serviceReport = {
        name: service.name,
        uri: service.baseUri,
        healthReport: new HealthReport('unhealthy', null),
        version: 'x.x.x.x'
      };
      this.serviceReport.push(serviceReport);
    }

    return serviceReport;
  }

  getEntriesKeys(service: ServiceReport): string[] {
    if (!service.healthReport) {
      return [];
    }
    return Object.keys(service.healthReport.entries);
  }

}

export class ServiceDescriptor {
  constructor(
    public name: string,
    public baseUri: string) {
  }
}

export class ServiceReport {
  constructor(
    public name: string,
    public uri: string,
    public healthReport: HealthReport,
    public version: string
  ) {
  }
}
