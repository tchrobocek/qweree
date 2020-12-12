import { Injectable } from '@angular/core';
import {HttpClient} from '@angular/common/http';
import {EnvironmentService} from '../environment/environment.service';
import {Observable} from 'rxjs';
import {ExplorerObject} from '../../model/cdn/ExplorerObject';
import {UriHelper} from '../UriHelper';
import {catchError} from 'rxjs/operators';

@Injectable({
  providedIn: 'root'
})
export class CdnAdapterService {

  constructor(
    private httpClient: HttpClient,
    private environment: EnvironmentService
  ) {
  }

  explore(path: string): Observable<ExplorerObject[]> {
    const explorerUri = UriHelper.getUri(this.environment.getEnvironment().cdn.baseUri, '/api/v1/explorer///');
    return this.httpClient.get<ExplorerObject[]>(UriHelper.getUri(explorerUri, path))
      .pipe(catchError(e => {throw e; }));
  }
}
