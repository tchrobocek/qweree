import { Injectable } from '@angular/core';
import {HttpClient} from '@angular/common/http';
import {EnvironmentService} from '../environment/environment.service';
import {Observable} from 'rxjs';
import {PathHelper} from '../PathHelper';
import {catchError, map} from 'rxjs/operators';
import {User} from '../../model/authentication/User';

@Injectable({
  providedIn: 'root'
})
export class IdentityAdapterService {

  constructor(
    private httpClient: HttpClient,
    private environmentService: EnvironmentService
  ) { }


  getUsers(skip: number, take: number, sortField: string, sortDirection: number): Observable<User[]> {
    const uri = PathHelper.getPath(this.environmentService.getEnvironment().authentication.baseUri, '/api/v1/identity/users?skip=' + skip + '&take=' + take + '&sort[' + sortField + ']=' + sortDirection);
    return this.httpClient.get<User[]>( uri, {observe: 'response'})
      .pipe(map(response => {
        return response.body;
      }), catchError(e => {
        throw e;
      }));
  }

  createUser(input: UserCreateInput): Observable<User> {
    const uri = PathHelper.getPath(this.environmentService.getEnvironment().authentication.baseUri, '/api/v1/identity/users');
    return this.httpClient.post<User>(uri, input).pipe(catchError(e => { throw e; }));
  }

  getUser(id: string): Observable<User> {
    const uri = PathHelper.getPath(this.environmentService.getEnvironment().authentication.baseUri, '/api/v1/identity/users/' + id);
    return this.httpClient.get<User>(uri).pipe(catchError(e => { throw e; }));
  }

  deleteUser(id: string): Observable<boolean> {
    const uri = PathHelper.getPath(this.environmentService.getEnvironment().authentication.baseUri, '/api/v1/identity/users/' + id);
    return this.httpClient.delete(uri).pipe(map(() => true), catchError(e => { throw e; }));
  }
}

export class UserCreateInput {

  constructor(
    public username: string,
    public contactEmail: string,
    public fullName: string,
    public password: string,
    public roles: string[]
  ) {
  }
}
