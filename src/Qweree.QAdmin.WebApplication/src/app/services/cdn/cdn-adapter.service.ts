import { Injectable } from '@angular/core';
import {HttpClient} from '@angular/common/http';
import {EnvironmentService} from '../environment/environment.service';
import {Observable} from 'rxjs';
import {ExplorerObject} from '../../model/cdn/ExplorerObject';
import {PathHelper} from '../PathHelper';
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

  store(path: string, mediaType: string, file: ArrayBuffer): Observable<string> {
    const uri = PathHelper.getPath(this.environment.getEnvironment().cdn.baseUri, '/api/v1/storage/');
    const options = {headers: {'Content-Type': mediaType}};
    return this.httpClient.post<string>(PathHelper.getPath(uri, path), file, options)
      .pipe(catchError(e => {throw e; }));
  }

  explore(path: string): Observable<ExplorerObject[]> {
    const explorerUri = PathHelper.getPath(this.environment.getEnvironment().cdn.baseUri, '/api/v1/explorer/');
    return this.httpClient.get<ExplorerObject[]>(PathHelper.getPath(explorerUri, path))
      .pipe(catchError(e => {throw e; }));
  }
}
