import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { of } from 'rxjs';
import { map } from 'rxjs/operators';
import { environment } from 'src/environments/environment';
import { Member } from '../_models/member';
import { PaginatedResult } from '../_models/pagination';
import { UserParams } from '../_models/userParams';
import { getPaginatedResult, getPaginationHeaders } from './paginationHelper';

const httpOptions = {
  headers: new HttpHeaders({
    Authorization: 'Bearer ' + JSON.parse(localStorage.getItem('user'))?.token
  })
}

@Injectable({
  providedIn: 'root'
})
export class MembersService {
  baseUrl = environment.apiUrl;
  members: Member[] = [];


  constructor(private http: HttpClient) { }

  getMembers(userparams: UserParams) {
    // if (this.members.length > 0) return of(this.members);
    // return this.http.get<Member[]>(this.baseUrl + '/user/').pipe(
    //   map(members => {
    //     this.members = members;
    //     return members;
    //   })
    // )
    let params = getPaginationHeaders(userparams.pageNumber, userparams.pageSize);
    params = params.append('minAge', userparams.minAge.toString());
    params = params.append('maxAge', userparams.maxAge.toString());
    params = params.append('gender', userparams.gender);
    params = params.append('orderBy', userparams.orderBy);

    return getPaginatedResult<Member[]>(this.baseUrl + '/user/', params, this.http);
  }

  getMember(username: string) {
    // const member = this.members.find(x => x.username === username);
    // if (member !== undefined) return of(member);
    return this.http.get<Member>(this.baseUrl + '/user/' + username);
  }

  updateMember(member: Member) {
    return this.http.put(this.baseUrl + '/user/', member).pipe(
      map(() => {
        const index = this.members.indexOf(member);
        this.members[index] = member;
      })
    )
  }

  setMainPhoto(photoId: number) {
    return this.http.put(this.baseUrl + '/user/set-main-photo/' + photoId, {});
  }

  deletePhoto(photoId: number) {
    return this.http.delete(this.baseUrl + '/user/delete-photo/' + photoId);
  }

  addLike(username: string) {
    return this.http.post(this.baseUrl + '/likes/' + username, {});
  }

  getLikes(predicate: string, pageNumber: number, pageSize: number) {
    let params = getPaginationHeaders(pageNumber, pageSize);
    params = params.append('predicate', predicate);
    // return this.http.get<Partial<Member[]>>(this.baseUrl + '/likes?predicate=' + predicate);
    return getPaginatedResult<Partial<Member[]>>(this.baseUrl + '/likes/', params, this.http);
  }

}
