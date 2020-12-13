import {Component, EventEmitter, Input, OnChanges, Output, SimpleChanges} from '@angular/core';
import {ExplorerDirectory, ExplorerFile} from '../../../../model/cdn/ExplorerObject';
import {CdnAdapterService} from '../../../../services/cdn/cdn-adapter.service';

@Component({
  selector: 'app-path-explorer',
  templateUrl: './path-explorer.component.html',
  styleUrls: ['./path-explorer.component.scss']
})
export class PathExplorerComponent implements OnChanges {
  @Input() public path: string;
  @Output() public pathChanged = new EventEmitter<string>();
  public directories: ExplorerDirectory[];
  public files: ExplorerFile[];

  constructor(
    private cdnAdapter: CdnAdapterService,
  ) {
    this.directories = [];
    this.files = [];
  }

  ngOnChanges(model: SimpleChanges){
    console.log('change path');
    console.log(model);
    this.files = [];
    this.directories = [];
    this.reload();
  }

  reload(): void {
    this.cdnAdapter.explore(this.path)
      .subscribe(objects => {
        objects.forEach(o => {
          const dir = (o as ExplorerDirectory);
          if (dir.totalCount) {
            this.directories.push(dir);
          }
          const file = (o as ExplorerFile);
          if (file.id) {
            this.files.push(file);
          }
        });
      });
  }

  changePath(path: string): void {
    this.pathChanged.emit(path);
  }

  getPrevPath(path: string): string {
    if (path.endsWith('/')) {
      path.substring(0, path.length - 1);
    }

    return path.substring(0, path.lastIndexOf('/'));
  }
}
