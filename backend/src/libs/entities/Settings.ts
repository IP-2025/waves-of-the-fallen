import { Column, Entity, JoinColumn, OneToOne, PrimaryColumn } from 'typeorm';



@Entity()
export class Settings {
  @PrimaryColumn({
    type: 'varchar',
    nullable: false,
    unique: true,
  })
  player_id!: string;

  @Column({
    type: 'double precision',
  })
  musicVolume!: number;

  @Column({
    type: 'double precision',
  })
  soundVolume!: number;

  @OneToOne(() => Settings, (settings) => settings.player_id)
  @JoinColumn()
  settings!: Settings;
}
