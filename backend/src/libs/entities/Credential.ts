import { Entity, PrimaryGeneratedColumn, Column, OneToOne, JoinColumn } from 'typeorm';
import { Player } from './Player';

@Entity()
export class Credential {
  @PrimaryGeneratedColumn('uuid')
  id!: string;

  @Column({
    type: 'varchar',
    nullable: true
  })
  email!: string;

  @Column({
    type: 'varchar',
    nullable: true
  })
  password!: string;

  @OneToOne(() => Player, player => player.credential)
  @JoinColumn()
  player!: Player;
}
