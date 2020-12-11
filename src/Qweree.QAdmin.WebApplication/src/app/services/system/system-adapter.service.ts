import {HttpClient} from '@angular/common/http';
import {HealthReport} from '../../model/system/HealthReport';
import {Observable} from 'rxjs';

export class SystemAdapterService {

  constructor(private httpClient: HttpClient, private uri: string) {
  }

  healthReport(): Observable<HealthReport> {
    return this.httpClient.get<HealthReport>(this.uri + '/api/v1/_system/health');
  }
}
